from django.shortcuts import render
from django.http import HttpResponse
from .models import History

def index(request):
    history_list = History.objects.order_by('id')[:10]
    context = {'history_list' : history_list}
    return render(request, 'index.html', context)

def search(request, location):
    response = "You're searching for the weather at {}".format(location)
    return HttpResponse(response)
