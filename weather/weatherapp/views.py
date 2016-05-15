from django.contrib.auth import authenticate, login
from django.contrib.auth.decorators import login_required
from django.shortcuts import render
from django.http import HttpResponse
from .models import History
from datetime import datetime
from datetime import timedelta

import requests

weatherUri = 'https://api.forecast.io/forecast/beec74efaeee64d55513d049112d385b'

class WeatherData:

    def __init__(self, time, tempMax, tempMin, summary):
        self.time = time
        self.tempMax = tempMax
        self.tempMin = tempMin
        self.summary = summary

    def __str__(self):
        return 'time: {} | tempMax: {} | tempMin: {} | summary: {}'.format(self.time, self.tempMax, self.tempMin, self.summary)

    def __repr__(self):
        return self.__str__();

@login_required(login_url='/weatherapp/accounts/login/')
def index(request):

    location = request.POST['location'] if 'location' in request.POST else None
    mapJson = None
    weatherJson = None
    dateAndWeather = []
    coord = getLocation(location)
    weatherList = getWeather(coord[1][0], coord[1][1])

    history_list = History.objects.filter(user=request.user).order_by('-id')[:10]

    if coord[0] == 0 and weatherList:
        h = History(user=request.user, search_history=location, search_result=dateAndWeather)
        h.save()

    context = {
            'history_list' : history_list,
            'location' : location,
            'location_result' : coord[0],
            'lat' : coord[1][0],
            'lng' : coord[1][1],
            'temperature_list_high' : [x.tempMax for x in weatherList],
            'temperature_list_low' : [x.tempMin for x in weatherList],
            'weather_conditions' : [{"date": x.time, "summary": x.summary} for x in weatherList],
            'date_list' : [x.time for x in weatherList]
            }

    return render(request, 'index.html', context)

# 0: Success
# 1: Could not find search
# 2: Input was null
def getLocation(location):

    if location:

        mapGetUri = 'https://maps.googleapis.com/maps/api/geocode/json?address={}&key=AIzaSyCLjAV18w2FPLa2z9vU2DgqK9cHe9h6J_s'.format(location)
        mapJson = requests.get(mapGetUri).json()

        if mapJson['status'] == 'OK':
            lat = mapJson['results'][0]['geometry']['location']['lat']
            lng = mapJson['results'][0]['geometry']['location']['lng']
            return (0, (lat, lng))
        else:
            return(1, (47, -122))

    return (2, (47, -122))

def getWeather(lat, lng):

    weatherGetUri= '{}/{},{}'.format(weatherUri, lat, lng)
    weatherReq = requests.get(weatherGetUri)

    if weatherReq.status_code == 200:

        weatherJson = weatherReq.json()
        weatherInfoList = [x for x in weatherJson['daily']['data']]
        weatherList = []

        for weatherInfo in weatherInfoList:

            weatherList.append(WeatherData(weatherInfo['time'], weatherInfo['temperatureMax'], weatherInfo['temperatureMin'], weatherInfo['summary']))

        today = weatherList[0].time
        weatherList = [getWeatherAtTime(lat, lng, x) for x in getLastThreeDays(today)] + weatherList

        for weatherData in weatherList:
            if weatherData.time == today:
                weatherData.time = "Today"
            else:
                weatherData.time = datetime.fromtimestamp(weatherData.time).strftime("%a %b/%d")

        return weatherList

    return None

def getWeatherAtTime(lat, lng, time):

    weatherGetUri = '{}/{},{},{}'.format(weatherUri, lat, lng, time)
    weatherReq = requests.get(weatherGetUri)

    if weatherReq.status_code == 200:

        weatherJson = weatherReq.json()
        weatherInfo = weatherJson['daily']['data'][0]
        return WeatherData(weatherInfo['time'], weatherInfo['temperatureMax'], weatherInfo['temperatureMin'], weatherInfo['summary'])

    return WeatherData(None, 0, 0, None)

def getLastThreeDays(time):

    dt = datetime.utcfromtimestamp(time)
    return [int((dt - timedelta(days=x)).timestamp()) for x in [3, 2, 1]]
