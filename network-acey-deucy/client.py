#!/usr/bin/python3

import sys
import os
import socket
import select
import time
import curses
import traceback
import random
import signal
import functools
import re
import asciicards

#### GLOBAL VARIABLES ####
global HOST # The server IP address
global PORT # The server port address
global SOCK # The server socket

global NAME # The prefered client name (and actual name later)
global MONEY # The amount of money the client has
global AUTO # Whether AI and auto is on or not
global TEXT # Whether additional text should be displayed or not
global RUN # Whether the client should keep running

global MSGI # Messages coming in from the server
global MSGO # Messages going out to the server
global MSGT # Half-formed server messages
global msgS # Skip to next start message
global msgE # Skip to current message end

global MAXW # Max size of the terminal window (width)
global MINH # Min size of the terminal window (height)
global W1 # Input window
global W2 # Output window (chats, etc)
global W3 # Game window (cards, etc)

global POT # Pot size
global PLAYERS # Players list
global GPOS # Client's game position, -1 if in lobby

global DECK # Our cardCounter
global CARD1
global CARD2
global PAUSE
global TIMER

# Player class for containing information about a player
##########################################################################
class player():

    def __init__(self, name, pos, money):
        
        self.name = name
        self.pos = pos
        self.money = money

# Deck class for counting cards
##########################################################################
class cardCounter():
    
    def __init__(self):
        
        self.deck = list(range(52)) * 2
        
    def remove(self, c):
        
        try:
            self.deck.remove(c)
        except:
            cprint("Card counting problem! Tried to remove {0}".format(c),
                   curses.A_BOLD)

    def probability(self, card1, card2):
        
        # Counts the number of winning cards in the deck
        def countCards(cardlist):
            count = 0
            for c in cardlist:
                count += self.deck.count(c)
            return count

        c1 = card1 % 13
        c2 = card2 % 13
        spread = abs(c1 - c2) - 1
        prob = []
        if c1 > c2:
            temp = c1
            c1 = c2
            c2 = temp

        try:
            # No possibility to win
            if spread == 0:
                prob.append(0.0)
            # Cards are two of a kind
            elif spread == -1:
                # Calculation 1
                wincards = [] 
                for i in list(range(0, c1))[0:]:
                    wincards.extend(list(range(i, 52, 13)))
                wincards = [c for c in wincards if c != 0]
                wcount = countCards(wincards)
                prob.append(wcount / len(self.deck))
                # Calculation 2
                wincards = []
                for i in list(range(c1, 13))[1:]:
                    wincards.extend(list(range(i, 52, 13)))
                wcount = countCards(wincards)
                prob.append(wcount / len(self.deck))
            # Normal calculation
            else:
                wincards = []
                for i in list(range(c1, c2))[1:]:
                    wincards.extend(list(range(i, 52, 13)))
                wcount = countCards(wincards)
                prob.append(wcount / len(self.deck))
        except:
            prob.append(0.0)

        return prob
        
    def shuffle(self):
        
        self.deck = list(range(52)) * 2

# Helper Functions
##########################################################################

# Splits a string according to the delimiter but keeps delimiter
def splitkeepsep(s, sep):
    mlist = functools.reduce(lambda acc,
                             elem: acc[:-1] + [acc[-1] + elem] 
                             if elem == sep else acc + [elem],
                             re.split("(%s)" % re.escape(sep), s), [])
    return [m for m in mlist if m != ""]

#### FUNCTION CALLS ####
##########################################################################

# Check for incoming messages, write them to MSGI
def inM():

    global SOCK
    global MSGI
    global MSGT
    global msgS
    global msgE
    
    r = select.select([SOCK], [], [], 0)[0]
    if r:
        data = r[0].recv(80)

        # Have data, add to MSGI list
        if data:
            
            # VALID MESSAGES FROM SERVER
            valid = list(range(32, 127))
            cmds = ["[JOIN|", "[STRT|", "[PLYR|", "[CAR1|", "[CAR3|",
                    "[GMOV|", "[CHAT|", "[STRK|", "[KICK|", "[SHUF]"]
            # If the server sent junk
            strike_server = False

            ###############################
            #### Loop through the data ####
            for i, byte in enumerate(data):
                
                # Ignore newlines and carriage returns
                if data[i] == 10 or data[i] == 13:
                    continue
                # Handle skip conditions
                if msgS:
                    if data[i] == 91:
                        strike_server = True
                        msgS = False
                    else:
                        continue
                if msgE:
                    if data[i] == 93:
                        strike_server = True
                        msgE = False
                    continue

                # Temp message found
                if MSGT != "":
                    # If at end of a message ']'
                    if data[i] == 93:
                        MSGT += chr(data[i])
                        if len(MSGT) < 6:
                            strike_server = True
                        elif MSGT[:6] not in cmds:
                            strike_server = True
                        else:
                            MSGI.append(MSGT)
                            MSGT = ""
                    # Malformed server message
                    elif data[i] == 91 or \
                            data[i] not in valid or \
                            len(MSGT) == 79:
                        MSGT = ""
                        msgE = True
                    # Normal character, add to message buffer
                    else:
                        MSGT += chr(data[i])

                # No temp message, check for '['
                else:
                    # If start of a message '['
                    if data[i] == 91:
                        MSGT += chr(data[i])
                    # Else skip to next proper message
                    else:
                        msgS = True

            #### Close for-loop of data ####
            ################################
                    
            # Notify self of a strike against the server
            if strike_server:
                curses.endwin()
                print("Server sent garbage, terminating")
                print(data)
                exit()

        # No data, server terminated connection
        else:
            curses.endwin()
            print("Server closed connection - socket error while read")
            exit()

# Send message m out through a socket, for debugging print on W2 as well
def outM():

    global MSGO
    global SOCK
    
    # If we can write and have a message, write out all messages
    w = select.select([], [SOCK], [], 0)[1]    
    if w and MSGO != []:
        try:
            SOCK.sendall("".join(MSGO).encode('ascii'))
            MSGO = []
        except socket.error as e:
            curses.endwin()
            print("Server closed connection - socket error while write")
            exit()

# User typed something, process it
def userM(msg):

    global RUN
    global AUTO
    global MSGO
    global TEXT
    global PLAYERS
    global CARD1
    global CARD2
    
    if msg == "!exit":
        RUN = False
    elif msg == "!auto":
        if AUTO:
            cprint("****Auto-play has been turned off.")
            AUTO = False
        else:
            cprint("****Auto-play has been turned on.")
            AUTO = True
    elif msg == "!text":
        if TEXT:
            TEXT = False
        else:
            TEXT = True
    elif msg == "!players":
        TEXT = True
        cprint(", ".join([p.name for p in PLAYERS]))
        TEXT = False
    elif msg == "!prob":
        save_text = TEXT
        TEXT = True
        if CARD1 != 52 and CARD2 != 52:
            prob = DECK.probability(CARD1, CARD2)
            # Normal in-between
            if len(prob) == 1:
                cprint("Winning probability: {0}".format(prob[0]))
            # Low or High (Low First, High Second)
            elif len(prob) == 2:
                cprint("Winning probability (low): {0}".format(prob[0]))
                cprint("Winning probability (high): {0}".format(prob[1]))
        else:
            cprint("No current probabilities")
        TEXT = save_text
    elif msg[0:4] == "!bet":
        TEXT = True
        msg = msg.split(' ')
        msg = [m for m in msg if m]
        try:
            betamount = int(msg[1])
            bettype = msg[2]
        except:
            betamount = None
            bettype = None
        valid = ['B', 'H', 'L']
        if len(msg) != 3 or betamount == None or bettype == None or \
                bettype not in valid:
            cprint("Invalid bet, type !help to see format", curses.A_DIM)
        else:
            MSGO.append("[BETS|{0}|{1}]".format(betamount, bettype))
        TEXT = False
    elif msg == "!help":
        TEXT = True
        cprint("Type !exit to exit this application")
        cprint("Type !text to toggle display of game state messages")
        cprint("Type !players to print all known players (includes kicked)")
        cprint("Type !prob to get the current winning probabilities")
        cprint("Type !bet <amount> <outcome> to place a bet")
        cprint("      outcome = 'B' - Between, 'L' - Low, 'H' - High")
        cprint("Type anything that begins with '[' to send a raw server msg (debug, see spec)")
        TEXT = False
    elif msg[0] == '[':
        msg = msg.upper()
        MSGO.append(msg)
    else:
        msg = msg.replace('[', ' ')
        msg = msg.replace(']', ' ')
        msg = "[CHAT|{0}]".format(msg)
        if len(msg) <= 80:
            MSGO.append(msg)
        else:
            cprint("****Message too long, cannot send.")

# Process our initial join and ignore subsequent joins
def processJOIN():
    
    global MSGI
    global NAME
    global MONEY
    global JOINED
    
    JOINED = False

    rm_join = []
    for m in [m for m in MSGI if "[JOIN|" in m]:
        if not JOINED:
            temp = m[1:-1].split('|')
            NAME = temp[1]
            MONEY = (int)(temp[2])
 
            cUserInfo()
            rm_join.append(m)
            JOINED = True
        else:
            curses.endwin()
            print("Server sent more than one JOIN message, terminating")
            exit()

    MSGI = [m for m in MSGI if m not in rm_join]

# Process chats
def processCHAT():

    global MSGI
    
    remove_chats = []
    for m in MSGI:
        if "[CHAT|" in m:
            temp = splitkeepsep(m[1:-1], '|')
            cprint("{0}: {1}".format(temp[1][:-1], "".join(temp[2:])), curses.A_BOLD)
            remove_chats.append(m)

    MSGI = [m for m in MSGI if m not in remove_chats]

def processSTRK():
    
    global MSGI

    reason = ["garbage", "wrong join", "bad bet message", "bad chat message",
             "invalid bet", "invalid chat"]

    rm_strike = []
    for m in MSGI:
        if "[STRK|" in m:
            temp = m[1:-1].split('|')
            left = (int)(temp[1])
            code = (int)(temp[2])
            cprint("****Server strikes you for {0} - strike count: {1}"
                   .format(reason[code], left), curses.A_DIM)
            rm_strike.append(m)

    MSGI = [m for m in MSGI if m not in rm_strike]

def processKICK():
    
    global PLAYERS
    global MSGI
    global RUN

    reason = ["too many strikes", "timeout", "bankrupcy"]

    rm_kick = []
    for m in MSGI:
        if "[KICK|" in m:
            temp = m[1:-1].split('|')
            pos = (int)(temp[1])
            code = (int)(temp[2])
            plyr = next((p for p in PLAYERS if p.pos == pos), None)
            if plyr is not None:
                cprint("****Server kicks {0} for {1}"
                       .format(plyr.name, reason[code]), curses.A_DIM)
                if plyr.name == NAME:
                    RUN = False
                    curses.endwin()
                    print("Server kicked")
            rm_kick.append(m)

    MSGI = [m for m in MSGI if m not in rm_kick]

# Deck has been shuffled
def processSHUF(m):
    
    global DECK

    DECK.shuffle()

# Start of a game, start keeping track of PLAYERS and CARDS
def processSTRT(m):
    
    global PLAYERS
    global POT
    global DECK

    POT = (int)(m[1:-1].split('|')[2])
    PLAYERS = []
    DECK.shuffle()

    cprint("****Game Starting!", curses.A_DIM)

# End of a game, reset PLAYERS, GPOS, POT, CARD1, CARD2, update MONEY
def processGMOV(m):

    global PLAYERS
    global MONEY
    global GPOS
    global POT
    global CARD1
    global CARD2

    m = m[1:-1].split('|')
    MONEY = (int)(m[2])
    GPOS = -1
    PLAYERS = []
    POT = 0
    CARD1 = 52
    CARD2 = 52

    cprint("****Game Over!", curses.A_DIM)    
    cRefresh()

# Put players into memory for GUI
def processPLYR(m):
    
    global PLAYERS
    global GPOS

    m = m[1:-1].split('|')
    new_player = player(m[1], (int)(m[2]), (int)(m[3]))
    PLAYERS.append(new_player)
    if NAME in m:
        GPOS = new_player.pos
        cUserInfo()

def processCAR1(m):

    def processAUTO(minbet):

        def howBet(prob, minbet):
            
            global POT
            global MONEY

            bet = minbet

            # Bet the entire pot if we have a 80% chance of winning
            if prob > 0.80:
                bet = POT
            # Bet the probability times the POT if we have 50-80% chance of winning
            elif prob > .50:
                bet = (int)(POT * prob)
            # Bet the minimum bet if we have less than 50% chance of winning
            else:
                pass
            # Handle half-pot deals
            if bet < minbet:
                bet = minbet
            # Handle bet more than player has
            if bet > MONEY:
                bet = MONEY
            return bet
        
        global DECK
        global CARD1
        global CARD2
        global POT

        spread = abs(CARD1 % 13 - CARD2 % 13) -1
        prob = DECK.probability(CARD1, CARD2)
        bet = 0
        btype = ''

        # Normal in-between
        if len(prob) == 1:
            bet = howBet(prob[0], minbet)
            btype = 'B'
        
        # Low or High (Low First, High Second)
        elif len(prob) == 2:
            if prob[0] > prob[1]:
                bet = howBet(prob[0], minbet)
                btype = 'L'
            else:
                bet = howBet(prob[1], minbet)
                btype = 'H'

        # Something funky occured
        else:
            curses.endwin()
            print("Wtf, how did we get here (processAUTO)")
            exit()

        MSGO.append("[BETS|{0}|{1}]".format(int(bet), btype))
    
    global PLAYERS
    global GPOS
    global CARD1
    global CARD2
    global AUTO
    global DECK

    m = m[1:-1].split('|')
    pos = (int)(m[1])
    minbet = (int)(m[2])
    CARD1 = int(m[3])
    CARD2 = int(m[4])

    DECK.remove(CARD1)
    DECK.remove(CARD2)

    # Update the MiniState of the game
    cMiniState(pos)
    
    plyr = next((p for p in PLAYERS if p.pos == pos), None)
    if plyr != None:
        cCAR1(plyr.name, plyr.money, minbet, CARD1, CARD2)
        if AUTO and plyr.pos == GPOS:
            processAUTO(minbet)

def processCAR3(m):
    
    global PLAYERS
    global MONEY
    global POT
    global DECK

    m = m[1:-1].split('|')
    pos = (int)(m[1])
    card3 = (int)(m[2])
    netgain = (int)(m[3])
    POT = (int)(m[4])
    playermoney = (int)(m[5])

    DECK.remove(card3)
        
    plyr = next((p for p in PLAYERS if p.pos == pos), None)
    if plyr != None:
        plyr.money = playermoney
        cCAR3(plyr.name, plyr.money, netgain, card3)
        if plyr.name == NAME:
            MONEY = playermoney
            cUserInfo()

# Process all incoming server messages
def process():

    global MSGI
    global PAUSE
    global TIMER

    # All these can be processed out of order
    processJOIN()
    processCHAT()
    processSTRK()
    processKICK()

    # Must be processed in order
    if PAUSE:
        if TIMER == 0:
            TIMER = time.time()
        elif time.time() - TIMER > 1:
            TIMER = 0
            PAUSE = False
    else:
        while MSGI != []:
            m = MSGI.pop(0) if MSGI else ""
            if "[SHUF]" in m:
                processSHUF(m)
            elif "[GMOV|" in m:
                processGMOV(m)
            elif "[STRT|" in m:
                processSTRT(m)
            elif "[PLYR|" in m:
                processPLYR(m)
            elif "[CAR1|" in m:
                processCAR1(m)
            elif "[CAR3|" in m:
                processCAR3(m)
            else:
                curses.endwin()
                print("What? How did we get to the end of the switch for processing MSGI?")
                exit()

#### CURSES HANDLING ####
##########################################################################

def cprint(msg, attr = 0):

    global W2
    global TEXT

    if attr != 0 or TEXT:
        W2.addstr(msg, attr)
        W2.refresh()
        if len(msg) != 80:
            W2.addstr("\n")

def cRefresh():

    global CARD1
    global CARD2
    
    cUserInfo()
    cCAR1("(None)", "0", 0, CARD1, CARD2)
    cCAR3(0, 0, None, 52)
    cMiniState(-1)

def cUserInfo():
    
    global W3
    global NAME
    global MONEY
    global GPOS

    game_pos = ""
    if GPOS == -1:
        game_pos = "In Lobby"
    else:
        game_pos = "Game Position: {0}".format(GPOS)

    W3.border(0)
    W3.addnstr(0, 0, "NAME: {0} | $$$: {1} | {2} | GUIs Are Overrated"
               .format(NAME, MONEY, game_pos), MAXW)
    cMiniState(-1)
    W3.refresh()
    
def cMiniState(pos):

    global PLAYERS
    global GPOS
    global W3

    currentTurn = False
    gpos = GPOS
    if GPOS == pos:
        currentTurn = True
    if 0 not in [p.pos for p in PLAYERS]:
        pos -= 1
        gpos -= 1

    for i in list(range(0, 80, 2)):
        W3.addnstr(H3 - 2, i, "0 ", 2)
    for i in range(0, len(PLAYERS) * 2, 2):
        W3.addnstr(H3 - 2, i, "P ", 2)

    if pos > -1:
        W3.addnstr(H3 - 2, pos * 2, "X ", 2)

    if GPOS > -1:
        if currentTurn:
            W3.addnstr(H3 - 2, pos * 2, "X ", 2,
                       curses.color_pair(1) | curses.A_BOLD)
        else:
            W3.addnstr(H3 - 2, gpos * 2, "P ", 2,
                       curses.color_pair(1) | curses.A_BOLD)

    W3.refresh()

def cCAR1(name, money, minbet, x, y):

    global W3
    global MAXW
    global POT

    W3.addnstr(2, 3, "Current Player:     ", 20)
    W3.addnstr(3, 3, "{0}                 ".format(name), 20)
    W3.addnstr(5, 3, "Player Money:       ", 20)
    W3.addnstr(6, 3, "{0}                 ".format(money), 20)
    W3.addnstr(8, 3, "Minimum Bet: {0}    ".format(minbet), 20)
    W3.addnstr(9, 3, "Pot Size: {0}       ".format(POT), 20)
    
    card1 = asciicards.get(x)
    for index in list(range(1, 10)):
        W3.addnstr(index, 30, card1[index-1], 13)
    card2 = asciicards.get(y)
    for index in list(range(1, 10)):
        W3.addnstr(index, 45, card2[index-1], 13)
    card3 = asciicards.get(52)
    for index in list(range(1, 10)):
        W3.addnstr(index, 64, card3[index-1], 13)
    W3.refresh()

def cCAR3(name, money, net, z):

    global W3
    global MAXW
    global POT
    global CARD1
    global CARD2
    global PAUSE

    if net != None:
        if net == 0:
            cprint("****{0} Passed Turn (Bet 0) | Cards: {1}{2}"
                   .format(name, [CARD1%13+1, CARD2%13+1], [z%13+1]))
        elif net > 0:
            cprint("****{0} Won! +${1} - total: {2} | Cards: {3}{4}"
                   .format(name, net, money,
                           [CARD1%13+1, CARD2%13+1], [z%13+1]))
        elif net < 0:
            cprint("****{0} Lost! -${1} - total: {2} | Cards: {3}{4}"
                   .format(name, abs(net), money,
                           [CARD1%13+1, CARD2%13+1], [z%13+1]))
        W3.addnstr(9, 3, "Pot Size: {0}       ".format(POT), 20)
        PAUSE = True

    card3 = asciicards.get(z)
    for index in list(range(1, 10)):
        W3.addnstr(index, 64, card3[index-1], 13)
    W3.refresh()

#### HANDLE SIGNALS ####
##########################################################################

def handle_sigint(signum, frame):
    
    curses.endwin()
    exit()

#### START OF MAIN PROGRAM ####
##########################################################################

if __name__ == "__main__":
    
    # SIGNALS
    signal.signal(signal.SIGTSTP, signal.SIG_IGN)
    signal.signal(signal.SIGINT, handle_sigint)

    # GLOBAL VARIABLES INITIALIZATION
    HOST = "localhost"
    PORT = 36744
    SOCK = None
    
    NAME = "Homura"
    MONEY = 0
    AUTO = True
    TEXT = True
    RUN = True

    MSGI = []
    MSGO = []
    MSGT = ""
    msgS = False
    msgE = False

    MAXW = 80
    MINH = 20
    W1 = None
    W2 = None
    W3 = None

    POT = 0
    PLAYERS = []
    GPOS = -1

    DECK = cardCounter()
    CARD1 = 52
    CARD2 = 52
    PAUSE = False
    TIMER = 0

    # COMMANDLINE ARGS
    assert len(sys.argv) < 9
    if "-a" in sys.argv:
        AUTO = True
        assert "-m" not in sys.argv
    if "-m" in sys.argv:
        AUTO = False
        assert "-a" not in sys.argv
    for index in range(len(sys.argv)):
        try:
            if sys.argv[index] == "-n":
                NAME = sys.argv[index+1]
            if sys.argv[index] == "-s":
                HOST = sys.argv[index+1]
            if sys.argv[index] == "-p":
                PORT = (int)(sys.argv[index+1])
        except:
            print("Invalid argv format")
            exit()

    # DO A CONNECT AND JOIN THING
    SOCK = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    SOCK.connect((HOST, PORT))
    MSGO.append("[JOIN|{0}]".format(NAME))

    # Initialize the curses library
    stdscr = curses.initscr()
    curses.start_color()
    curses.init_pair(1, curses.COLOR_CYAN, curses.COLOR_BLACK)

    # VALUES:
    # W3 = 15 height (constant)
    # W2 = 1 height min (variable) (ssize[0] - (H3 + H1))
    # W1 = 1 height (constant)
    H3 = 14
    H1 = 1
    H2 = H3 + H1 # Need to subtract this from ssize[0]

    ssize = (0, 0)

    try:

        ########################
        #### STRT MAIN LOOP ####
        temp_str = ""
        valid_chars = list(range(32, 127))
        while RUN:

            # Sleep client
            time.sleep(0.05)
            # Stop everything if display is too small
            if stdscr.getmaxyx()[0] < MINH or stdscr.getmaxyx()[1] < MAXW:
                stdscr.clear()
                stdscr.addstr("Display too small")
                stdscr.refresh()
                continue
            # Handle resizing of the terminal window
            if ssize != stdscr.getmaxyx():
                # Get new screen size
                ssize = stdscr.getmaxyx()
                W3 = curses.newwin(H3, MAXW, 0, 0)
                W2 = curses.newwin(ssize[0] - H2, MAXW, H2 - 1, 0)
                W1 = curses.newwin(H1, MAXW, ssize[0] - 1, 0)

                W3.attrset(curses.A_BOLD)

                W1.nodelay(1)
                W2.scrollok(True)
                W1.leaveok(0)
                W2.leaveok(1)
                W3.leaveok(1)
                W1.addch(0, 0, '>', curses.A_BOLD)
                if temp_str == "":
                    W1.addstr("Type !help for commands")
                    W1.move(0, 1)
                cRefresh()
            
            # Get input from the screen, send to userM
            char = W1.getch()
            if char == 10 and temp_str != "":
                W1.clear()
                W1.addch(0, 0, '>', curses.A_BOLD)
                userM(temp_str)
                temp_str = ""
            elif char == 127:
                W1.clear()
                W1.addch(0, 0, '>', curses.A_BOLD)
                temp_str = temp_str[:-1]
                W1.addnstr(0, 1, temp_str, MAXW - 2)
            elif char in valid_chars:
                W1.clear()
                W1.addch(0, 0, '>', curses.A_BOLD)
                temp_str += chr(char)
                W1.addnstr(0, 1, temp_str, MAXW - 2)
            W1.refresh()

            # Do a game thing
            inM()
            process()
            outM()
        #### END MAIN LOOP ####
        #######################

        # Stop the curses library
        curses.endwin()

    except:

        # Stop the curses library
        curses.endwin()
        print("DEBUG <actual cards> <card value>", [CARD1, CARD2], [CARD1%13, CARD2%13])
        print("DEBUG <what's left in MSGI>", MSGI)
        print("DEBUG <players in game>", [(p.name, p.pos) for p in PLAYERS])
        print("DEBUG <GPOS>", GPOS)
        traceback.print_exc()
