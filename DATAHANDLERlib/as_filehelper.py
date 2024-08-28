# -*- coding: utf-8 -*-
"""
Created on Sun Aug 11 15:22:52 2024

@author: adria
"""

import os

class FileHelper:
    @staticmethod
    def file_exists(file_path: str) -> bool:
        """Verifica si un archivo existe en la ruta especificada."""
        if not file_path:
            raise ValueError("El path del archivo no puede ser null o vacío.")
        return os.path.exists(file_path)

    @staticmethod
    def create_file(file_path: str, content: str) -> None:
        """Crea un nuevo archivo en la ruta especificada con el contenido dado."""
        if not file_path:
            raise ValueError("El path del archivo no puede ser null o vacío.")
        with open(file_path, 'w') as file:
            file.write(content)

    @staticmethod
    def read_file(file_path: str) -> str:
        """Lee el contenido de un archivo en la ruta especificada."""
        if not file_path:
            raise ValueError("El path del archivo no puede ser null o vacío.")
        if not FileHelper.file_exists(file_path):
            raise FileNotFoundError(f"El archivo no se encuentra en la ruta especificada: {file_path}")
        with open(file_path, 'r') as file:
            return file.read()

    @staticmethod
    def delete_file(file_path: str) -> None:
        """Elimina el archivo en la ruta especificada."""
        if not file_path:
            raise ValueError("El path del archivo no puede ser null o vacío.")
        if not FileHelper.file_exists(file_path):
            raise FileNotFoundError(f"El archivo no se encuentra en la ruta especificada: {file_path}")
        os.remove(file_path)
