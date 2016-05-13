from django.contrib.auth import authenticate, login
from django.contrib.auth.decorators import login_required
from django.shortcuts import render
from django.http import HttpResponse
from .models import History

import requests
import datetime

@login_required(login_url='/weatherapp/accounts/login/')
def index(request):

    location = request.POST['location'] if 'location' in request.POST else None
    mapJson = None
    weatherJson = None
    dateAndWeather = []

    if location:

        mapUri = 'https://maps.googleapis.com/maps/api/geocode/json?address={}&key=AIzaSyCLjAV18w2FPLa2z9vU2DgqK9cHe9h6J_s'.format(location)
        mapJson = requests.get(mapUri).json()

        if mapJson['status'] == 'OK':

            lat = mapJson['results'][0]['geometry']['location']['lat']
            lng = mapJson['results'][0]['geometry']['location']['lng']

            weatherUri = 'https://api.forecast.io/forecast/beec74efaeee64d55513d049112d385b/{},{}'.format(lat, lng)
            weatherJson = requests.get(weatherUri).json()

            dateAndWeather = getWeather(weatherJson)

        h = History(user=request.user, search_history=location, search_result=mapJson)
        h.save()

    history_list = History.objects.order_by('-id')[:10]

    context = {
                'history_list' : history_list,
                'location' : location,
                'mapJson' : mapJson,
                'weatherJson' : weatherJson,
                'temperature_list' : [x['temperatureMax'] for x in dateAndWeather],
                'date_list' : [datetime.datetime.fromtimestamp(x['time']).strftime("%A") for x in dateAndWeather]
              }
    return render(request, 'index.html', context)

def getWeather(weatherJson):

   dateAndWeather = [x for x in weatherJson['daily']['data']]

   return dateAndWeather
