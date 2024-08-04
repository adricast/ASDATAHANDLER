using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASDATAHANDLER.Utils
{
  
        public static class FileHelper
        {
            /// <summary>
            /// Verifica si un archivo existe en la ruta especificada.
            /// </summary>
            /// <param name="filePath">La ruta del archivo.</param>
            /// <returns>Devuelve true si el archivo existe, de lo contrario, false.</returns>
            public static bool FileExists(string filePath)
            {
                if (string.IsNullOrEmpty(filePath))
                    throw new ArgumentException("El path del archivo no puede ser null o vacío.", nameof(filePath));

                return File.Exists(filePath);
            }

            /// <summary>
            /// Crea un nuevo archivo en la ruta especificada con el contenido dado.
            /// </summary>
            /// <param name="filePath">La ruta del archivo.</param>
            /// <param name="content">El contenido que se va a escribir en el archivo.</param>
            /// <exception cref="ArgumentException">Si el path del archivo es null o vacío.</exception>
            public static void CreateFile(string filePath, string content)
            {
                if (string.IsNullOrEmpty(filePath))
                    throw new ArgumentException("El path del archivo no puede ser null o vacío.", nameof(filePath));

                File.WriteAllText(filePath, content);
            }

            /// <summary>
            /// Lee el contenido de un archivo en la ruta especificada.
            /// </summary>
            /// <param name="filePath">La ruta del archivo.</param>
            /// <returns>El contenido del archivo.</returns>
            /// <exception cref="ArgumentException">Si el path del archivo es null o vacío.</exception>
            /// <exception cref="FileNotFoundException">Si el archivo no existe.</exception>
            public static string ReadFile(string filePath)
            {
                if (string.IsNullOrEmpty(filePath))
                    throw new ArgumentException("El path del archivo no puede ser null o vacío.", nameof(filePath));

                if (!FileExists(filePath))
                    throw new FileNotFoundException("El archivo no se encuentra en la ruta especificada.", filePath);

                return File.ReadAllText(filePath);
            }

            /// <summary>
            /// Elimina el archivo en la ruta especificada.
            /// </summary>
            /// <param name="filePath">La ruta del archivo.</param>
            /// <exception cref="ArgumentException">Si el path del archivo es null o vacío.</exception>
            /// <exception cref="FileNotFoundException">Si el archivo no existe.</exception>
            public static void DeleteFile(string filePath)
            {
                if (string.IsNullOrEmpty(filePath))
                    throw new ArgumentException("El path del archivo no puede ser null o vacío.", nameof(filePath));

                if (!FileExists(filePath))
                    throw new FileNotFoundException("El archivo no se encuentra en la ruta especificada.", filePath);

                File.Delete(filePath);
            }
        }
    }


