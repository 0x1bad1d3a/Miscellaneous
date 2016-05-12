from django.contrib.auth import authenticate, login
from django.contrib.auth.decorators import login_required
from django.shortcuts import render
from django.http import HttpResponse
from .models import History

import requests

@login_required(login_url='/weatherapp/accounts/login/')
def index(request):

    location = request.POST['location'] if 'location' in request.POST else None
    mapJson = None
    weatherJson = None

    if location:

        mapUri = 'https://maps.googleapis.com/maps/api/geocode/json?address={}&key=AIzaSyCLjAV18w2FPLa2z9vU2DgqK9cHe9h6J_s'.format(location)
        mapJson = requests.get(mapUri).json()

        if mapJson['status'] == 'OK':

            lat = mapJson['results'][0]['geometry']['location']['lat']
            lng = mapJson['results'][0]['geometry']['location']['lng']

            weatherUri = 'https://api.forecast.io/forecast/beec74efaeee64d55513d049112d385b/{},{}'.format(lat, lng)
            weatherJson = requests.get(weatherUri).json()

        h = History(user=request.user, search_history=location, search_result=mapJson)
        h.save()

    history_list = History.objects.order_by('-id')[:10]

    context = {
                'history_list' : history_list,
                'location' : location,
                'mapJson' : mapJson,
                'weatherJson' : weatherJson
              }
    return render(request, 'index.html', context)

def login(request):
    username = request.POST['username']
    password = request.POST['password']
    user = authenticate(username=username, password=password)
    if user is not None and user.is_active:
        login(request, user)
        return redirect('index')
    else:
        return HttpResponse("Error logging in")

def search(request, location):
    context = { 'location' : location }
    return render(request, 'search.html', context)
