# -*- coding: utf-8 -*-
"""
Created on Sun Aug 11 15:21:11 2024

@author: adria
"""

import json

class JsonHandler:
    @staticmethod
    def serialize_object(obj):
        return json.dumps(obj, indent=4)

    @staticmethod
    def deserialize_object(json_str):
        return json.loads(json_str)

    @staticmethod
    def serialize_object_to_file(obj, file_path):
        with open(file_path, 'w') as file:
            json.dump(obj, file, indent=4)

    @staticmethod
    def deserialize_object_from_file(file_path):
        with open(file_path, 'r') as file:
            return json.load(file)
