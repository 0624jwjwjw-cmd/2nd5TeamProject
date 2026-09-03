using UnityEngine;

public class GradeDatabase : MonoBehaviour
{
    public static GradeDatabase Instance { get; private set; }

    [SerializeField] private GradeData[] grades;
    public int GradeCount => grades.Length;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public GradeData GetGrade(int grade)
    {
        if (grade <= 0 || grade > grades.Length)
        {
            return null;
        }

        return grades[grade - 1];
    }
}