#!/usr/bin/python3

# Server python program for Acey Duecy

# Yay imports, do the hard work for me!
# import antigravity
import sys
import os
import socket
import select
import signal
import time
import curses
import functools
import re
import pickle
import random
import errno
import traceback

# This is so ugly... Python makes it too easy to do things the wrong way

# Global variables - oh god this is ugly as fuck
# Oh well ┐(￣ー￣)┌
##########################################################################
global server # server socket
global users # list of user (class)
global players # list of player (class)
global datafile # string - datafile name

global min_players # int
global lobby_timeout # time
global lobby_timeout_set # time
global play_timeout # time

global deck # cardHandler
global cards # cards
global game_pos # game pos ALWAYS POSITIVE (0 = no game)
global min_b # int - minimum bet
global min_a # int - minimum ante
global pot # monies

global W1 # input window
global W2 # output window
global RUN # the main run loop
global PORT # Port of the server (for curses)
global DLEVEL # Debug level for text output
##########################################################################

# Player class for containing information about a player
##########################################################################
class player():

    def __init__(self, p_socket):
        self.p_socket = p_socket # Player socket
        self.p_socket_addr = None # Saved player socket address (for debug)
        self.p_name = None # Player name
        self.p_money = 1000 # Player money balance
        self.p_msg = [] # Player message list (properly parsed)
        self.p_msg_skip_s = False # Skip-to-start in parsing messages
        self.p_msg_skip_e = False # Skip-to-end in parsing messages
        self.p_msg_junk = 0 # Counter to keep track of number of junk chars
        self.p_msg_temp = "" # String for storing half-formed messages
        self.p_msg_out = [] # Outgoing messages to be sent for this player
        self.p_strike = 0 # Number of strikes on the player
        self.p_kick = False # If the player is flagged to be kicked
        self.p_pos = -1 # Player play position (-1 for lobby), otherwise > 0
        self.p_timeout = 0 # Player play timeout
        self.p_bet = False # Flags if the player is allowed to bet

# Users known to the server
##########################################################################
class user():
    
    def __init__(self, user_name, user_money):
        self.user_name = user_name
        self.user_money = user_money

# Helper Functions
##########################################################################

# Splits a string according to the delimiter but keeps delimiter
def splitkeepsep(s, sep):
    mlist = functools.reduce(lambda acc,
                             elem: acc[:-1] + [acc[-1] + elem] 
                             if elem == sep else acc + [elem],
                             re.split("(%s)" % re.escape(sep), s), [])
    return [m for m in mlist if m != ""]

# Connection module
##########################################################################
class ConnectionHandler():

    # Initialize the server and set it to listen
    def start(P):

        global server
        global PORT

        PORT = P

        # Create the socket and start listening
        try:
            server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            server.bind(('', P))
            server.setblocking(False)
            server.listen(5)
            print("Setting up server on %s port %s" % ('localhost', P))
        except socket.error:
            print("Port already in use, try another port.")
            exit(0)

    # Kills the server and runs any cleanup tasks
    def stop():
        
        global server
        global players
        global RUN

        cprint("NOTIFY: Shutting down server... please wait", curses.A_BOLD)

        FileHandler.update()
        for p in players:
            p.p_socket.shutdown(socket.SHUT_RDWR)
            p.p_socket.close()

        server.shutdown(socket.SHUT_RDWR)
        server.close()

        FileHandler.write()
        RUN = False

    # Check for incoming connections and adds them to the players list
    # Kills the connection if the size of players list is over 256
    def openConn():

        global players

        (read, write, excep) = select.select([server], [], [], 0)

        # Incoming connection!
        for my_socket in read:
                
            (new_connection, client_address) = my_socket.accept()
            new_connection.setblocking(0)
            cprint("EVENT: New connection from {0}".format(client_address),
                   curses.color_pair(3))
            
            new_player = player(new_connection)
            new_player.p_socket_addr = new_connection.getpeername()

            if len(players) < 256:
                players.append(new_player)
                cprint("EVENT: Number of players now {0}".format(len(players)),
                       curses.color_pair(3))
            else:
                cprint("WARNING: Max players reached, closing {0}"
                       .format(new_player.p_socket_addr),
                       curses.color_pair(2))
                new_player.p_socket.close()

    # Close sockets of bad clients
    def closeConn():
        
        global players

        FileHandler.update()
        close_list = [p for p in players if p.p_kick]
        players = [p for p in players if p not in close_list]

        for p in close_list:
            p.p_socket.close()
    
    # Checks for messages from players and writes them to their p_msg list
    def inMessages():

        global players
        
        inputs = [p.p_socket for p in players]
        (read, write, excep) = select.select(inputs, [], [], 0)
        inputs = [p for p in players if p.p_socket in read]

        # VALID MESSAGES FROM CLIENT
        valid = list(range(32, 127))

        for p in inputs:

            data = p.p_socket.recv(80)
            
            # If there's data, add it to player's msg list
            # Append to player's p_msg
            if data:

                strike_list = []

                # Loop through the data and do simple pre-processing
                for i, byte in enumerate(data):

                    # Ignore newlines and carriage returns
                    if data[i] == 10 or data[i] == 13:
                        continue

                    # Handle skip conditions
                    if p.p_msg_skip_s:
                        if data[i] == 91:
                            p.p_msg_junk = 0
                            strike_list.append(0)
                            p.p_msg_skip_s = False
                        else:
                            p.p_msg_junk += 1
                            continue
                    if p.p_msg_skip_e:
                        if data[i] == 93:
                            p.p_msg_junk = 0
                            p.p_msg_skip_e = False
                            continue
                        else:
                            p.p_msg_junk +=1
                            continue

                    # temp message found
                    if p.p_msg_temp != "":
                        
                        # If at end of a message ']'
                        if data[i] == 93:
                            # Strike client for sending garbage
                            if not re.match("\[JOIN\||\[CHAT\||\[BETS\|",
                                            p.p_msg_temp):
                                strike_list.append(0)
                            # Strike client for empty bets or chats
                            elif len(p.p_msg_temp) == 6:
                                if "[JOIN|" in p.p_msg_temp:
                                    strike_list.append(1)
                                elif "[CHAT|" in p.p_msg_temp:
                                    strike_list.append(3)
                                elif "[BETS|" in p.p_msg_temp:
                                    strike_list.append(2)
                            # Good message... for now, add to msg list
                            else:
                                p.p_msg_temp += chr(data[i])
                                p.p_msg.append(p.p_msg_temp)
                            p.p_msg_temp = ""

                        # If we hit another '['
                        # or character is not printable valid
                        # or p_msg_temp is too long
                        # then we need to strike
                        elif data[i] == 91 or \
                                data[i] not in valid or \
                                len(p.p_msg_temp) == 79:
                            # Append strike types to strike list
                            if "[JOIN|" in p.p_msg_temp:
                                strike_list.append(1)
                            elif "[CHAT|" in p.p_msg_temp:
                                if len(p.p_msg_temp) == 79:
                                    strike_list.append(3)
                                else:
                                    strike_list.append(5)
                            elif "[BETS|" in p.p_msg_temp:
                                strike_list.append(2)
                            else:
                                strike_list.append(0)
                            # Clear message buffer, and skip to the end
                            p.p_msg_temp = ""
                            p.p_msg_skip_e = True
                        
                        # Normal character, append to message buffer
                        else:
                            p.p_msg_temp += chr(data[i])
                    
                    # No temp message, check for '['
                    else:
                        
                        # If start of a message '['
                        if data[i] == 91:
                            p.p_msg_temp += chr(data[i])
                        # Else skip to the next proper message
                        else:
                            p.p_msg_junk += 1
                            p.p_msg_skip_s = True
                # End for-loop of data

                # DEBUG
                cprint("GAME: MSG_IN {0} {1}".format(p.p_name, p.p_msg))
                
                # Send out strikes
                if p.p_msg_junk > 80:
                    strike_list.append(0)
                for s in strike_list:
                    p.p_strike += 1
                    strike_msg = "[STRK|{0}|{1}]".format(p.p_strike,s)
                    p.p_msg_out.append(strike_msg)
                    cprint("WARNING: STRIKE bad msg {0} {1}"
                           .format(p.p_socket_addr, strike_msg),
                           curses.color_pair(2))

            # No data, socket has been terminated on other side
            else:

                # Strike out player and build kick message
                p.p_kick = True
                kick_msg = "[KICK|{0}|1]".format(p.p_pos)

                # Send KICK message to all applicable players
                if p.p_pos != -1:
                    # for players in list whose not the disconnected player
                    for pp in [pp for pp in players if pp.p_name != None]:
                        pp.p_msg_out.append(kick_msg)

                cprint("ERROR: Remote disconnect while read {0} {1} {2}"
                       .format(p.p_socket_addr, p.p_name, kick_msg),
                       curses.color_pair(1))
                
    def outMessages():
        
        global players
        
        # If you have a message and your socket can write, you can send!
        have_msg = [p for p in players if p.p_msg_out != []]
        msg_socket = [p.p_socket for p in have_msg]
        (read, write, excep) = select.select([], msg_socket, [], 0)
        can_send = [p for p in have_msg if p.p_socket in write]

        for p in can_send:

            # Reformat outMessage if the player is flagged to be kicked
            if p.p_kick:
                s_count = 0
                remove = []
                for m in p.p_msg_out:
                    if "[STRK|" in m:
                        strike_list = m.split('|')
                        if (int)(strike_list[1]) > 3:
                            remove.append(m)
                p.p_msg_out = [m for m in p.p_msg_out if m not in remove]
            
            # cprint("GAME: MSG_OUT {0} {1}".format(p.p_name, p.p_msg_out))

            try:
                p.p_socket.sendall("".join(p.p_msg_out).encode('ascii'))
                p.p_msg_out = []
            except socket.error as e:
                if e.errno == errno.EPIPE or e.errno == 104:
                    cprint("ERROR: Remote disconnect while write {0} {1}"
                           .format(p.p_socket_addr, p.p_name),
                           curses.color_pair(1))
                else:
                    raise

# Handles game logic
class GameHandler():

    # Kicks offensive players from the game
    def mark_bad_players():
        
        global players
        global min_a
        global min_b

        now = time.time()

        strike_list1 = [p for p in players if p.p_strike >= 3]
        strike_list2 = [p for p in players if p.p_timeout != 0
                        and now - p.p_timeout > play_timeout]
        strike_list3 = [p for p in players if p.p_money <= min_b + min_a]
        strike_list = strike_list1 + strike_list2 + strike_list3
        
        for p in [p for p in players if p in strike_list]:

            reason = None # SHOULD NOT HAPPEN
            if p in strike_list1:
                reason = 0
            if p in strike_list2:
                reason = 1
            if p in strike_list3:
                reason = 2
            kick_msg = "[KICK|{0}|{1}]".format(p.p_pos, reason)

            # Send KICK message to all applicable players
            # Flag the player for kicking
            p.p_kick = True
            p.p_msg_out.append(kick_msg)
            if p.p_pos != -1:
                for pp in [pp for pp in players
                           if pp.p_name is not None
                           and pp is not p]:
                    pp.p_msg_out.append(kick_msg)

            # Debug print
            cprint("WARNING: KICK {0} from server {1} {2}"
                   .format(p.p_name, p.p_socket_addr, kick_msg),
                   curses.color_pair(2))

    # Assigns usernames, and strikes bad JOINs
    def processUsernames():
        
        global users
        global players
        
        # Find players with no name and has a message
        no_name = [p for p in players if p.p_name is None]
        name_msg = [p for p in no_name if p.p_msg != []]

        # Process those players
        for p in name_msg:
            
            msg = p.p_msg.pop(0)
            msg = msg[1:-1].split('|')

            # Strike player if their first message is not a join
            # Strike player if JOIN message is malformed
            if msg[0] != "JOIN" or \
            len(msg) > 2 or \
            len(msg[1]) > 16 or \
            not re.match('^[a-zA-Z0-9]+$', msg[1]):
                cprint("WARNING: STRIKE bad join {0}"
                       .format(p.p_socket_addr), curses.color_pair(2))
                p.p_strike += 1
                p.p_msg_out.append("[STRK|{0}|1]".format(p.p_strike))
                continue

            # If we get here we HAVE to give them a name ****
            name = msg.pop(1)
            same_name = [pp for pp in players if pp.p_name == name]

            # If no common name, give name, else modify to be unique
            if same_name == []:
                p.p_name = name
            else:
                while same_name != []:
                    name = name + str(random.randrange(10))
                    if len(name) > 16:
                        name = name[1:]
                    same_name = [pp for pp in players if pp.p_name == name]
                p.p_name = name    
            
            # Restore player balance if we are aware of the player
            match_user = [u for u in users if u.user_name == name]
            if len(match_user) > 1:
                cprint("ERROR: match_user error (should not happen!)",
                       curses.color_pair(1)) # Should not happen
            elif len(match_user) == 0:
                pass # Default money amount
            else:
                p.p_money = match_user.pop().user_money

            # Send message to client & print debug message
            cprint("EVENT: Player registered as '{0}' {1}"
                   .format(p.p_name, p.p_socket_addr), curses.color_pair(3))
            p.p_msg_out.append("[JOIN|{0}|{1}|{2}]"
                               .format(p.p_name,p.p_money,players.index(p)))

            # Send them the game_start messages
            gamers = [p for p in players if p.p_pos != -1]
            game_start_msg = ""
            for g in gamers:
                g_msg = "[PLYR|{0}|{1}|{2}]".format(g.p_name,
                                                    g.p_pos, g.p_money)
                game_start_msg = game_start_msg + g_msg
            p.p_msg_out.append(game_start_msg)

        # Strike players that try to JOIN twice
        for p in players:
            rm_list = []
            for m in p.p_msg:
                if "[JOIN|" in m:
                    cprint("WARNING: STRIKE join after join {0} {1}"
                           .format(p.p_socket_addr, p.p_name),
                           curses.color_pair(2))
                    p.p_strike += 1
                    p.p_msg_out.append("[STRK|{0}|1]".format(p.p_strike))
                    rm_list.append(m)
            p.p_msg = [m for m in p.p_msg if m not in rm_list]
    
    # Do a chat thing
    # Server must re-parse chat messages that become too long on name add
    def processChats():
        
        global players

        # If player has a chat message, broadcast it
        for p in [p for p in players if p.p_msg != []]:

            rm_list = []
            for m in p.p_msg:

                if "[CHAT|" in m:

                    # Strike invalid chats and continue
                    if not re.match('^[ -~]+$', m):
                        cprint("WARNING: STRIKE bad chat {0} {1} {2}"
                               .format(p.p_socket_addr, p.p_name, m),
                               curses.color_pair(2))
                        p.p_strike += 1
                        strk_msg = "[STRK|{0}|5]".format(p.p_strike)
                        p.p_msg_out.append(strk_msg)
                        p.p_msg.remove(m)
                        continue
                    
                    # Put user's name in their chat
                    actual_msg = m.split('|')
                    actual_msg.insert(1, p.p_name)
                    actual_msg = "|".join(actual_msg)

                    # If chat length goes over 80 characters, split
                    # message into two
                    if len(actual_msg) > 80:
                        temp = actual_msg[79:]
                        temp = "|".join(["[CHAT", p.p_name, temp])
                        actual_msg = actual_msg[:79]
                        actual_msg = actual_msg + "]" + temp
                    
                    # Add the chat to all players p_msg_out
                    for pp in [pp for pp in players if pp.p_name != None]:
                        pp.p_msg_out.append(actual_msg)

                    # Is a broadcast, so print to console
                    cprint("NOTIFY: CHAT {0} {1}"
                           .format(p.p_name, actual_msg), curses.A_BOLD)

                    # Add to the remove list
                    rm_list.append(m)

            # Remove the message
            p.p_msg = [m for m in p.p_msg if m not in rm_list]
                        

    # Ok, time to game
    def processGame():

        # Helper function that starts a game
        def startGame():
            
            global players
            global game_pos
            global pot
            global min_a
            
            game_pos = 1
            gamers = []
            max_players = 40
            
            # Extract gamers, give them p_pos, and then put at end of list
            for p in players:
                if p.p_name is not None and max_players > 0:
                    gamers.append(p)
                    max_players -= 1
            players = [p for p in players if p not in gamers]
            random.shuffle(gamers)
            for i, p in enumerate(gamers):
                p.p_pos = i+1
                p.p_money -= min_a
                pot += min_a  
            players = players + gamers

            # Logistics update
            for p in [p for p in players if p.p_name != None]:
                p.p_msg_out.append("[STRT|{0}|{1}]".format(min_a, pot))

            # Send the game start messages sequence
            gamers = [p for p in players if p.p_pos != -1]
            game_start_msg = "" # Build game start msg in one go
            for g in gamers:
                g_msg = "[PLYR|{0}|{1}|{2}]".format(g.p_name,
                                                    g.p_pos, g.p_money)
                game_start_msg = game_start_msg + g_msg
            for p in [p for p in players if p.p_name != None]:
                p.p_msg_out.append(game_start_msg)

            cprint("GAME: Game Starting!", curses.color_pair(4))

        # Bye bye
        def stopGame():
            
            global players
            global game_pos
            global pot
            global cards
            global lobby_timeout_set

            gamers_list = [p for p in players if p.p_pos != -1]
            if len(gamers_list) == 1:
                gamers_list[0].p_money += pot

            game_pos = 0
            pot = 0
            cards = []
            lobby_timeout_set = 0
            deck.shuffle()

            # Put gamers back in the lobby (end)
            sort_players = []
            for p in players:
                if p.p_pos != -1:
                    p.p_pos = -1
                    p.p_timeout = 0
                    sort_players.append(p)
                    p.p_msg_out.append("[GMOV|{0}|{1}]"
                                       .format(players.index(p), p.p_money))
            players = [p for p in players if p not in sort_players]
            players.extend(sort_players)

            cprint("GAME: Game has ended!", curses.color_pair(4))

        # Run one play of the game
        def runGame():

            global players
            global game_pos
            global cards
            global pot            
            
            # Get the current player in the game
            # If current player has been kicked from the server -
            # Get the new current player
            current_player = [p for p in players if p.p_pos == game_pos]
            if not current_player:
                g_list = [p.p_pos for p in players if p.p_pos != -1]
                g_list.sort()
                if game_pos > max(g_list):
                    game_pos = g_list[0]
                else:
                    for g in g_list:
                        if game_pos < g:
                            game_pos = g
                            break
                current_player = [p for p in players if p.p_pos
                                  == game_pos]
            assert len(current_player) == 1
            current_player = current_player.pop()
                
            # If player does not have a timeout then give one
            # Also send the player their first two cards during this time
            if current_player.p_timeout == 0:

                current_player.p_timeout = time.time()
                current_player.p_bet = True
                    
                # Get two cards from the deck
                pcard1 = deck.getCard()
                pcard2 = deck.getCard()
                cards = [pcard1, pcard2]
                actual_bet = min_b
                if pot < min_b:
                    actual_bet = pot

                card_msg = "[CAR1|{0}|{1}|{2}|{3}]" \
                    .format(game_pos, actual_bet, cards[0], cards[1])
                for p in [p for p in players if p.p_name is not None]:
                    p.p_msg_out.append(card_msg)

                # Print out the card_msg to console
                cprint("GAME: CARDS {0}".format(card_msg), curses.color_pair(4))
                    
            # Check if the player has a bet to make
            else:
                
                # Check bets
                rm_msg = []
                for m in current_player.p_msg:
                    
                    # Valid bet, process and break
                    if checkBets(m):

                        # Get the bet and bet type and set some variables
                        player_bet = (int)(m[1:-1].split('|')[1])
                        player_BHL = m[1:-1].split('|')[2]
                        player_results = 0 # Positive = win, Negative = loss

                        # Draw a card and get values
                        card3 = deck.getCard()
                        card3_value = card3 % 13
                        cards_value = [c % 13 for c in cards]
                        cards_value.sort()
                        
                        # if first two card values are the same
                        if cards_value[0] == cards_value[1]:

                            assert player_BHL == 'H' or player_BHL == 'L'

                            # Player win/loss
                            if player_BHL == 'H':
                                if card3_value > cards_value[0]:
                                    player_results = 1
                                else:
                                    player_results = -1
                            else: # player_BHL = 'L'
                                if card3_value < cards_value[0]:
                                    player_results = 1
                                else:
                                    player_results = -1
                        
                        # else it's a regular in-between find
                        else:

                            assert player_BHL == 'B'
                            
                            # Player win/loss
                            if card3_value in \
                                    list(range(cards_value[0],
                                               cards_value[1]))[1:]:
                                    player_results = 1
                            else:
                                player_results = -1
                            
                        # Player special lose conditions
                        if card3_value == 0:
                            player_results = -1
                            if 0 in cards_value:
                                player_results = -4
                        elif card3_value in cards_value:
                            player_results = -2
                                
                        # Cleanup and process the results
                        assert player_results != 0

                        amount_won = player_bet * player_results
                        pot = pot - amount_won
                        current_player.p_money = current_player.p_money + \
                            amount_won
                        game_msg = "[CAR3|{0}|{1}|{2}|{3}|{4}]" \
                            .format(game_pos, card3,
                                    amount_won, pot,
                                    current_player.p_money)

                        for p in [p for p in players if p.p_name != None]:
                            p.p_msg_out.append(game_msg)

                        cprint("GAME: CARDS {0}".format(game_msg), curses.color_pair(4))
                        
                        current_player.p_bet = False
                        current_player.p_timeout = 0
                        game_pos += 1
                        cards = []
                        rm_msg.append(m)
                        break
                    # Invalid bet, strike and continue
                    else:
                        current_player.p_strike += 1
                        s_msg="[STRK|{0}|4]".format(current_player.p_strike)
                        current_player.p_msg_out.append(s_msg)
                        cprint("WARNING: STRIKE bad bet message {0} {1}"
                               .format(current_player.p_name, m),
                               curses.color_pair(2))
                        rm_msg.append(m)
                
                # Clear all processed bets
                for m in rm_msg:
                    current_player.p_msg.remove(m)

        # Helper function that checks that the bet isn't invalid
        def checkBets(m):

            global min_b
            global cards
            global pot

            valid_type = ['B', 'H', 'L']
            m = m[1:-1].split('|')

            try:
                # Return false if length is not 3, not a int, not valid type
                # Has to be length 3 bet message
                # Bet can't be below min bet
                # Bet can't be above pot
                # Bet type has to be valid
                b = (int)(m[1])
                if len(m) != 3 or \
                        b < min_b and ((min_b < pot) or (b < pot)) or \
                        b > pot or \
                        m[2] not in valid_type: 
                    return False
                # Return false if the B/H/L type is wrong based on cards
                cards_value = [c % 13 for c in cards]
                if cards_value[0] == cards_value[1]:
                    if m[2] == 'B':
                        return False
                else:
                    if m[2] == 'H' or m[2] == 'L':
                        return False
                return True
            except ValueError:
                return False

        # ------------------------------------------------------

        global players
        global game_pos
        global lobby_timeout
        global lobby_timeout_set

        # If p_bet flag != True, strike any clients sending a bet msg
        for p in [p for p in players if not p.p_bet and p.p_name != None]:
            if p.p_msg != []:
                for i in range(len(p.p_msg)):
                    p.p_msg.pop()
                    p.p_strike += 1
                    p.p_msg_out.append("[STRK|{0}|{1}]"
                                       .format(p.p_strike, 4))
                    cprint("WARNING: STRIKE bet out of turn {0} {1}"
                           .format(p.p_socket_addr, p.p_name), curses.color_pair(2))

        # No game in progress, check if we can start a game ****
        if game_pos == 0:
            p_count = [p for p in players if p.p_name is not None]
            if len(p_count) >= min_players:
                curr_time = time.time()
                if lobby_timeout_set == 0:
                    lobby_timeout_set = curr_time
                    cprint("NOTIFY: Min players reached starting {0}s countdown"
                          .format(lobby_timeout), curses.A_BOLD)
                if curr_time - lobby_timeout_set > lobby_timeout:
                    startGame()
            else:
                if lobby_timeout_set != 0:
                    lobby_timeout_set = 0
                    cprint("NOTIFY: Canceling game, resetting min players countdown", curses.A_BOLD)

        # Game in progress, we need to process the game ****
        else:

            gamers = [p for p in players if p.p_pos != -1]

            # Check if the game needs to stop
            if pot <= 0 or len(gamers) < 2:
                stopGame()

            # If the game doesn't need to stop, process the game
            else:
                runGame()

# Class that deals out the cards, will update [SHUF] message when needed
class cardHandler():
    
    def __init__(self):
        
        self.deck = list(range(52)) * 2
        random.shuffle(self.deck)

    def getCard(self):
        
        if len(self.deck) == 3:
            self.deck = list(range(52)) * 2
            random.shuffle(self.deck)
            for p in [p for p in players if p.p_name != None]:
                p.p_msg_out.append("[SHUF]")
        
        return self.deck.pop()

    def shuffle(self):
        
        self.deck = list(range(52)) * 2
        random.shuffle(self.deck)

# File handling module, handle reads and writes to the "database" file
##########################################################################
class FileHandler():
    
    # Read user data from file to users list
    def read():

        global datafile
        global users
    
        try:
            myfile = open(datafile, 'rb')
            users = pickle.load(myfile)
        except:
            print("Error loading player data, will be overwritten")

    # Write data from users list to file 
    def write():

        global datafile
        global users

        myfile = open(datafile, 'wb+')
        try:
            pickle.dump(users, myfile)
        except:
            print("Error writing player data")
            
    # Update the users list for writing
    def update():
        
        global users
        global players
        
        p_list = [p for p in players if p.p_name is not None]
        name_list = [p.p_name for p in players if p.p_name is not None]
        users = [u for u in users if u.user_name not in name_list]

        for p in p_list:
            new_user = user(p.p_name, p.p_money)
            users.append(new_user)

# Curses and related code
##########################################################################

def cprint(line, attr = 0):
    
    global W2
    global DLEVEL
    
    attr_dlevel = 5
    if attr == curses.color_pair(1):
        attr_dlevel = 1
        attr |= curses.A_BOLD
    elif attr == curses.color_pair(2):
        attr_dlevel = 2
        attr |= curses.A_BOLD
    elif attr == curses.color_pair(3):
        attr_dlevel = 3
        attr |= curses.A_BOLD
    elif attr == curses.A_BOLD:
        attr_dlevel = 4
    elif attr == -1: # For !help message
        attr_dlevel = -1
        attr = curses.A_BOLD
    
    if attr_dlevel <= DLEVEL:
        W2.addstr(line, attr)
        W2.refresh()
        if len(line) != 80:
            W2.addstr("\n")

def userM(line):

    global PORT
    global DLEVEL
    global players

    if line == "!help":
        cprint("**** Type !exit to gracefully shutdown the server", -1)
        cprint("**** Type !port to get the port of this server", -1)
        cprint("**** Type !users to see a list of known users", -1)
        cprint("**** Type !players to see a list of registered players", -1)
        cprint("**** Type !kick <name> to manually kick a player", -1)
        cprint("**** Type !dlevel <level> to set debug output [1 - 5]", -1)
        cprint("****       1: ERRORS only (red)", -1)
        cprint("****       2: WARNINGS and above (yellow)", -1)
        cprint("****       3: EVENTS and above (cyan)", -1)
        cprint("****       4: NOTIFICATIONS and above (white)", -1)
        cprint("****       5: All debug messages", -1)
        cprint("**** Note: dlevel has been set to 4", -1)
        DLEVEL = 4
    elif line == "!exit":
        ConnectionHandler.stop()
    elif line == "!port":
        cprint("**** Server is on port {0}".format(PORT), -1)
    elif line == "!users":
        for u in users:
            cprint("**** USER {0} ${1}"
                   .format(u.user_name, u.user_money), -1)
    elif line == "!players":
        for p in players:
            cprint("**** PLAYER {0}: ${1} STRIKE: {2} POS: {3}"
                  .format(p.p_name, p.p_money, p.p_strike, p.p_pos), -1)
    elif line[:5] == "!kick":
        line = line.split(' ')
        line = [l for l in line if l != '']
        for p in players:
            if p.p_name == line[1]:
                p.p_kick = True
                cprint("**** Kicked {0} from the server (Manual)"
                       .format(p.p_name), -1)
    elif line[:7] == "!dlevel":
        line = line.split(' ')
        line = [l for l in line if l != '']
        valid_d = [1, 2, 3, 4, 5]
        try:
            if int(line[1]) not in valid_d:
                cprint("**** Invalid DLEVEL", -1)
            else:
                DLEVEL = int(line[1])
        except:
            cprint("**** Invalid DLEVEL", -1)
    else:
        cprint("**** Invalid command, type !help for list of commands", -1)

# Start of main program
##########################################################################
if __name__ == "__main__":

    # Handle signals... yea, ignore everything
    signal.signal(signal.SIGINT, signal.SIG_IGN)
    signal.signal(signal.SIGPIPE, signal.SIG_IGN)
    signal.signal(signal.SIGTSTP, signal.SIG_IGN)

    # Global variables
    users = []
    players = []
    datafile = "playerdata"

    min_players = 2
    lobby_timeout = 0
    lobby_timeout_set = 0
    play_timeout = 60

    deck = cardHandler() # Our deck to pull cards from
    cards = [] # Cards for a player
    game_pos = 0
    min_a = 5
    min_b = 10
    pot = 0

    RUN = True
    DLEVEL = 5

    # Process commandline arguments
    if len(sys.argv) > 7:
        print("Invalid argv format")
        exit(1)
    for index in range(len(sys.argv)):
        try:
            if sys.argv[index] == '-m':
                min_players = (int)(sys.argv[index+1])
            if sys.argv[index] == '-l':
                lobby_timeout = (int)(sys.argv[index+1])
            if sys.argv[index] == '-t':
                play_timeout = (int)(sys.argv[index+1])
        except:
            print("Invalid argv format")
            exit(1)

    # Launch server, read users from file
    FileHandler.read()
    ConnectionHandler.start(36744)

    # Initialize curses and define parameters (don't need to be global)
    stdscr = curses.initscr()
    curses.start_color()
    curses.init_pair(1, curses.COLOR_RED, curses.COLOR_BLACK)
    curses.init_pair(2, curses.COLOR_YELLOW, curses.COLOR_BLACK)
    curses.init_pair(3, curses.COLOR_CYAN, curses.COLOR_BLACK)
    curses.init_pair(4, curses.COLOR_GREEN, curses.COLOR_BLACK)
    ssize = (0, 0)
    
    # Loop until we kill server
    temp_str = ""
    valid_chars = list(range(32, 127))
    while RUN:
        
        # SLEEP SERVER... NO NEED TO LOOP SO HARD
        time.sleep(0.1)

        # Initialize curses windows (or re-initialize on re-size)
        if ssize != stdscr.getmaxyx():
            ssize = stdscr.getmaxyx()
            W2 = curses.newwin(ssize[0] - 1, 80, 0, 0)
            W1 = curses.newwin(1, 80, ssize[0] - 1, 0)
            W2.scrollok(True)
            W1.nodelay(1)
            W1.addch(0, 0, '>', curses.A_BOLD)
            if temp_str == "":
                W1.addstr("Type !help for commands")
                W1.move(0, 1)
            W1.refresh()
            W2.refresh()

        try:
            # Send messages and kick players
            ConnectionHandler.outMessages()
            ConnectionHandler.closeConn()
            # Open connection and get messages
            ConnectionHandler.openConn()
            ConnectionHandler.inMessages()
            # Game logic
            GameHandler.processUsernames()
            GameHandler.processChats()
            GameHandler.processGame()
            GameHandler.mark_bad_players() # Must be done last
        # Last line of defense against crashes
        except socket.error as e:
            cprint("ERROR: Catching socket.error {0}".format(e.errno),
                   curses.color_pair(1))
        except select.error as e:
            cprint("ERROR: Catching select.error {0}".format(e.errno),
                   curses.color_pair(1))

        # Curses W1 handling
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
            W1.addnstr(0, 1, temp_str, 70)
        elif char in valid_chars:
            W1.clear()
            W1.addch(0, 0, '>', curses.A_BOLD)
            temp_str += chr(char)
            W1.addnstr(0, 1, temp_str, 70)
        W1.refresh()

    # Close the curses library
    curses.endwin()
