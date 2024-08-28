class CsvModel:
    def __init__(self, values):
        self.values = values

    def __str__(self):
        return ", ".join(self.values)
# -*- coding: utf-8 -*-

