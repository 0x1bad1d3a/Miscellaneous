from django.db import models
from django.contrib.auth.models import User

# Create your models here.

class History(models.Model):

    class Meta:
        verbose_name_plural = "Histories"

    user = models.ForeignKey(User)
    search_history = models.TextField()
    search_result = models.TextField()

    def __str__(self):
        return "user: {} | search_history: {} | search_result: {}".format(self.user, self.search_history, self.search_result)
