import re
from typing import List
from .csv_model import CsvModel

class SqlEngine:
    def execute_query(self, data: List[CsvModel], query: str) -> List[CsvModel]:
        if not data:
            return []

        headers = ['fecha', 'hora', 'tipo', 'entrada', 'salida', 'tipo_usuario', 'nombre_usuario']

        selected_columns = self.get_selected_columns(query)
        where_clause = self.get_where_clause(query)

        if '*' not in selected_columns and any(col.lower() not in [h.lower() for h in headers] for col in selected_columns):
            return []

        filtered_data = [record for record in data if self.evaluate_condition(record, where_clause, headers)]

        projected_data = [self.project_columns(record, selected_columns, headers) for record in filtered_data]

        return projected_data

    def evaluate_condition(self, record: CsvModel, condition: str, headers: List[str]) -> bool:
        """
        Evalúa una condición de la cláusula WHERE en un registro CSV.
        """
        if not condition:
            return True

        # Convertir la condición a minúsculas para manejo insensible a mayúsculas/minúsculas
        condition = condition.lower()

        # Dividir las condiciones en partes con 'and'
        and_conditions = condition.split(' and ')
        return all(self.evaluate_or_condition(record, cond.strip(), headers) for cond in and_conditions)

    def evaluate_or_condition(self, record: CsvModel, condition: str, headers: List[str]) -> bool:
        """
        Evalúa una condición que puede contener cláusulas or.
        """
        # Dividir las condiciones en partes con 'or'
        or_conditions = condition.split(' or ')
        return any(self.evaluate_single_condition(record, cond.strip(), headers) for cond in or_conditions)

    def evaluate_single_condition(self, record: CsvModel, condition: str, headers: List[str]) -> bool:
        """
        Evalúa una sola condición.
        """
        # Normaliza el operador y los valores
        pattern = r'(\w+)\s*(=|>|<|>=|<=|like)\s*(\d+|\'[^\']*\'|"[^"]*")'
        match = re.match(pattern, condition, re.IGNORECASE)
        if not match:
            return False

        column_name = match.group(1).lower()
        operator = match.group(2).lower()
        value = match.group(3).strip('\'"')

        try:
            column_index = headers.index(column_name)
        except ValueError:
            return False

        record_value = record.values[column_index].strip() if record.values else ''

        try:
            record_value_as_int = int(record_value)
            value_as_int = int(value)
        except ValueError:
            record_value_as_int = value_as_int = None

        if operator == '=':
            result = record_value.lower() == value.lower()
        elif operator == '>':
            result = record_value_as_int is not None and value_as_int is not None and record_value_as_int > value_as_int
        elif operator == '<':
            result = record_value_as_int is not None and value_as_int is not None and record_value_as_int < value_as_int
        elif operator == '>=':
            result = record_value_as_int is not None and value_as_int is not None and record_value_as_int >= value_as_int
        elif operator == '<=':
            result = record_value_as_int is not None and value_as_int is not None and record_value_as_int <= value_as_int
        elif operator == 'like':
            pattern = '^' + re.escape(value).replace('%', '.*') + '$'
            result = re.match(pattern, record_value, re.IGNORECASE) is not None
        else:
            result = False

        return result

    def project_columns(self, record: CsvModel, selected_columns: List[str], headers: List[str]) -> CsvModel:
        """
        Proyecta las columnas seleccionadas del registro CSV.
        """
        projected_values = [record.values[headers.index(col.lower())] if col.lower() in [h.lower() for h in headers] else '' for col in selected_columns]
        return CsvModel(projected_values)

    def get_selected_columns(self, query: str) -> List[str]:
        """
        Obtiene las columnas seleccionadas de la consulta SQL.
        """
        match = re.search(r'SELECT\s+(.*?)\s+FROM', query, re.IGNORECASE)
        if match:
            columns = match.group(1).split(',')
            return [col.strip() for col in columns]
        return []

    def get_where_clause(self, query: str) -> str:
        """
        Obtiene la cláusula WHERE de la consulta SQL.
        """
        # Convierte toda la consulta a minúsculas para evitar problemas con palabras clave
        query = query.lower()
        
        match = re.search(r'where\s+(.*?)\s*(order by|group by|$)', query, re.IGNORECASE)
        if match:
            return match.group(1)  # La cláusula WHERE ya está en minúsculas
        return ''

