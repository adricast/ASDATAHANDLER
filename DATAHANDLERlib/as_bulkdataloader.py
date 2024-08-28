import json
import xml.etree.ElementTree as ET
from dataclasses import dataclass, asdict
from typing import List, Type

@dataclass
class CsvModel:
    # Define los atributos de tu CsvModel aquí
    attribute1: str
    attribute2: int
    # Agrega otros atributos según tu necesidad

class BulkDataLoader:
    def load_from_json(self, json_file_path: str, model_class: Type[CsvModel]) -> List[CsvModel]:
        try:
            with open(json_file_path, 'r') as file:
                json_data = json.load(file)
                data = [model_class(**item) for item in json_data]
                return data
        except Exception as ex:
            print(f"Error al cargar datos desde JSON: {ex}")
            return []

    def load_from_xml(self, xml_file_path: str, model_class: Type[CsvModel]) -> List[CsvModel]:
        try:
            tree = ET.parse(xml_file_path)
            root = tree.getroot()
            
            data = []
            for item in root.findall('.//CsvModel'):
                attributes = {child.tag: child.text for child in item}
                data.append(model_class(**attributes))
            return data
        except Exception as ex:
            print(f"Error al cargar datos desde XML: {ex}")
            return []
# -*- coding: utf-8 -*-

