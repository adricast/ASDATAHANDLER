# -*- coding: utf-8 -*-
"""
Created on Sun Aug 11 15:31:31 2024

@author: adria
"""

import os
import csv
import json
from typing import List
from dataclasses import dataclass
from .as_jsonhandler import JsonHandler
from .as_bulkdataloader import BulkDataLoader
from .as_sqlengine import SqlEngine
from .as_filehelper import FileHelper
from .csv_model import CsvModel
@dataclass


class CsvService:
    
    def __init__(self, file_path: str = None):
        self.file_path = file_path

    def load_data(self):
        if self.file_path is None:
            raise ValueError("No se ha especificado la ruta del archivo.")
        if not FileHelper.file_exists(self.file_path):
            raise FileNotFoundError(f"El archivo no se encuentra en la ruta especificada: {self.file_path}")
        
        data = []
        with open(self.file_path, 'r', newline='') as file:
            reader = csv.reader(file)
            for row in reader:
                data.append(row)
        return data
    
    
    def read_csv(self, file_path: str) -> List[CsvModel]:
        """Lee un archivo CSV y devuelve una lista de CsvModel."""
        if not os.path.isfile(file_path):
            raise FileNotFoundError(f"El archivo {file_path} no se encontró.")
    
        result = []
        with open(file_path, 'r', newline='', encoding='utf-8') as file:
            reader = csv.reader(file)
            for row in reader:
                #print(f"Leyendo fila: {row}")  # Agrega esta línea para depuración
                if row:  # Verifica que la fila no esté vacía
                    result.append(CsvModel(values=row))
                else:
                    print("Fila vacía encontrada.")
        return result
    
    def write_csv(self, file_path: str, data: List[CsvModel]):
        """Escribe datos en un archivo CSV."""
        with open(file_path, 'w', newline='', encoding='utf-8') as file:
            writer = csv.writer(file)
            for record in data:
                #print(f"Escribiendo fila: {record.values}")  # Agrega esta línea para depuración
                writer.writerow(record.values)

    def create_csv_with_headers(self, file_path: str, headers: List[str], data: List[CsvModel]):
        """Crea un archivo CSV con encabezados y datos."""
        with open(file_path, 'w', newline='', encoding='utf-8') as file:
            writer = csv.writer(file)
            writer.writerow(headers)
            for record in data:
                writer.writerow(record.values)

    def export_to_json(self, csv_file_path: str, json_file_path: str):
        """Exporta datos de un archivo CSV a JSON."""
        csv_data = self.read_csv(csv_file_path)
        JsonHandler.serialize_object_to_file([record.values for record in csv_data], json_file_path)

    def validate_csv(self, file_path: str, delimiter: str = ','):
        """Valida que el archivo CSV tenga el mismo número de columnas en todas las filas."""
        with open(file_path, 'r', newline='', encoding='utf-8') as file:
            reader = csv.reader(file, delimiter=delimiter)
            headers = next(reader)
            for row in reader:
                if len(row) != len(headers):
                    raise ValueError("El número de columnas no coincide con el encabezado.")

    def read_csv_with_quotes(self, file_path: str, delimiter: str = ',', quotechar: str = '"') -> List[CsvModel]:
        """Lee un archivo CSV con comillas y delimitadores especificados."""
        result = []
        with open(file_path, 'r', newline='', encoding='utf-8') as file:
            reader = csv.reader(file, delimiter=delimiter, quotechar=quotechar)
            for row in reader:
                result.append(CsvModel(values=row))
        return result

    def write_csv_in_chunks(self, file_path: str, data: List[CsvModel], delimiter: str = ','):
        """Escribe datos en un archivo CSV en chunks, especificando el delimitador."""
        with open(file_path, 'w', newline='', encoding='utf-8') as file:
            writer = csv.writer(file, delimiter=delimiter)
            for record in data:
                writer.writerow(record.values)

    def execute_sql_query(self, file_path: str, query: str) -> List[CsvModel]:
        from .as_sqlengine import SqlEngine
        data = self.read_csv(file_path)
        
        # Imprimir los datos leídos para depuración
        #print("Datos leídos:")
        #for record in data:
        #    print(record.values)
        
        #print(f"Consulta SQL: {query}")
        
        query_engine = SqlEngine()
        result = query_engine.execute_query(data, query)
        
        # Imprimir el resultado de la consulta
        #print("Resultado de la consulta:")
        for row in result:
            print(row)  # Imprime las filas obtenidas
        
        return [CsvModel(values=row) for row in result]


    def load_data_from_xml(self, xml_file_path: str) -> List[CsvModel]:
        """Carga datos desde un archivo XML."""
        bulk_loader = BulkDataLoader()
        data = bulk_loader.load_from_xml(xml_file_path)
        return [CsvModel(values=record) for record in data]
