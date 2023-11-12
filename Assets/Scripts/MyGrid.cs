using UnityEngine;

public class MyGrid<TGridObject>
{
    private int width, height;
    private float cellSize;
    private Vector3 originPosition;
    private TGridObject[,] gridArray;
    public bool isShowDebug {get; set;}
    
    public MyGrid(int width, int height, float cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        gridArray = new TGridObject[width, height];
        if (isShowDebug)
            ShowDebug();
    }

     private void ShowDebug()
     {
         TextMesh[,] debugTextArray = new TextMesh[this.width, this.height];
         for (int i = 0; i < gridArray.GetLength(0); i++)
         {
             for (int j = 0; j < gridArray.GetLength(1); j++)
             {
                 debugTextArray[i, j] = CreateWorldText(gridArray[i, j].ToString(),
                                                     null,
                                                 GetWorldPosition(i, j) + new Vector3(cellSize,cellSize)*0.5f,
                                                     10,
                                                         Color.white,
                                                         TextAnchor.MiddleCenter);
                 
                 Debug.DrawLine(
                     GetWorldPosition(i,j), 
                     GetWorldPosition(i, j + 1),
                     Color.white, 100f);
                 Debug.DrawLine(
                     GetWorldPosition(i,j), 
                     GetWorldPosition(i + 1, j),
                     Color.white, 100f);
             }
    
             Debug.DrawLine(
                 GetWorldPosition(0, height),
                 GetWorldPosition(width, height),
                 Color.white, 100f);
             Debug.DrawLine(
                 GetWorldPosition(width, 0),
                 GetWorldPosition(width, height),
                 Color.white, 100f);
         }
     }
    
    public static TextMesh CreateWorldText(
        string text,
        Transform parent,
        Vector3 localPosition,
        int fontSize,
        Color color,
        TextAnchor textAnchor)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        gameObject.transform.SetParent(parent, false);
        gameObject.transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        return textMesh;
    }

    private Vector3 GetWorldPosition(int x, int y) 
        => new Vector3(x, y) * cellSize + originPosition;

    public void SetValue(int x, int y, TGridObject value)
    {
        if (!IsBelongToGrid(x, y)) return;
        gridArray[x, y] = value;
    }

    private bool IsBelongToGrid(int x, int y) 
        => x >= 0 && y >= 0 && x < width && y < height;

    public void SetValue(Vector3 worldPosition, TGridObject value)
    {
        (int x, int y) getXY = GetXY(worldPosition);
        SetValue(getXY.x, getXY.y, value);
    }
    
    private (int x, int y) GetXY(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos - originPosition).x / cellSize);
        int y = Mathf.FloorToInt((worldPos - originPosition).y / cellSize);
        return (x, y);
    }

    public TGridObject GetValue(int x, int y) 
        => IsBelongToGrid(x, y) ? gridArray[x, y] : default(TGridObject);
    
    public TGridObject GetValue(Vector3 worldPosition)
    {
        (int x, int y) getXY = GetXY(worldPosition);
        return GetValue(getXY.x, getXY.y);
    }
}
