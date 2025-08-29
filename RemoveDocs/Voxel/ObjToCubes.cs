using UnityEngine;
using System.Collections.Generic;

namespace Mikalai2006.Voxel
{
    public class ObjToCubes : MonoBehaviour
    {
        public string objFilePath; // Путь к OBJ файлу

        void Start()
        {
            // 1. Загрузка модели
            GameObject model = LoadOBJ(objFilePath);

            // 2. Обработка модели и создание кубов
            ProcessModelAndCreateCubes(model);
        }

        GameObject LoadOBJ(string filePath)
        {
            // Здесь будет логика загрузки OBJ файла, используя API Unity
            // (пример: MeshFilter meshFilter = GetComponent<MeshFilter>();)
            // и создания GameObject с MeshFilter и MeshRenderer
            return gameObject;  //new GameObject(); // Заглушка
        }

        void ProcessModelAndCreateCubes(GameObject model)
        {
            // Пример: получение Mesh от MeshFilter
            MeshFilter meshFilter = model.GetComponent<MeshFilter>();
            if (meshFilter == null) return;
            Mesh mesh = meshFilter.mesh;

            // Получение вершин и граней
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            // Создание кубов для каждой грани
            for (int i = 0; i < triangles.Length; i += 3)
            {
                // Создание куба
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                // Получение координат вершин грани
                Vector3 v1 = vertices[triangles[i]];
                Vector3 v2 = vertices[triangles[i + 1]];
                Vector3 v3 = vertices[triangles[i + 2]];

                // Расчет центра грани и нормали
                Vector3 center = (v1 + v2 + v3) / 3f;
                Vector3 normal = Vector3.Cross(v2 - v1, v3 - v1).normalized;

                // Позиционирование и поворот куба
                cube.transform.position = model.transform.TransformPoint(center);
                //cube.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal); // Пример вращения
                cube.transform.parent = this.transform; // Добавление куба к текущему GameObject

                // Текстурирование (пример)
                MeshRenderer renderer = cube.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    // Присвоение материала (загруженного из файла)
                    // renderer.material = yourMaterial; // Пример
                }
            }
        }
    }
}
