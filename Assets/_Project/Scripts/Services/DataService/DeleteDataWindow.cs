#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace TodoBoard
{
    public class DeleteDataWindow : EditorWindow
    {
        private string fileName = "save";

        [MenuItem("Tools/Data/Delete Save")]
        public static void ShowWindow()
        {
            GetWindow<DeleteDataWindow>("Delete Save Data");
        }

        private void OnGUI()
        {
            GUILayout.Label("Удаление сохранённого файла", EditorStyles.boldLabel);

            fileName = EditorGUILayout.TextField("Имя файла", fileName);

            if (GUILayout.Button("Удалить файл"))
            {
                if (DataService.DeleteData(fileName))
                {
                    Debug.Log($"Файл '{fileName}' успешно удалён.");
                }
                else
                {
                    Debug.LogWarning($"Файл '{fileName}' не найден или не удалось удалить.");
                }
            }
        }
    }
}
#endif
