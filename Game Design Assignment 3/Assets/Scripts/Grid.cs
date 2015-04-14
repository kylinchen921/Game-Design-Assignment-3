using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;


public class Grid : MonoBehaviour
{
    private int Width;
    private int Height;
    public float CellSize;
    public float CellSpacing = 0.1f;
    public GameObject FixedCell;
    public GameObject MoveableCell;
    public GameObject EmptyCell;
    public GameObject ChainPushableCell;
    public GameObject ExitCell;
	public GameObject ResetCel;
    public GameObject PlayerCell;
    public GameObject CellParent;
    
    /// use to group the cells
    private static int CellId = 0;

    [SerializeField]
    public ICell[,] cells;


    [SerializeField] 
    public Serialiseable2DArray I; 

    // Use this for initialization
	void Start ()
	{
        //ReadLevel("Level1");
	    //LoadObjectsToGrid();
	}

    private void LoadObjectsToGrid()
    {
        var objects = FindObjectsOfType<GameObject>();
        var iCells =
            objects.Where(o => o.GetComponent<ICell>() != null)
                .ToList()
                .ConvertAll<ICell>(o => o.GetComponent<ICell>());
        Rect[,] rects;
        if (cells == null) { 
            var maxX = iCells.Aggregate((i1,
                i2) =>
                (Math.Abs(i1.GameObject.transform.position.x) > Math.Abs(i2.GameObject.transform.position.x) ? i1 : i2)).GameObject.transform.position.x;
            var maxZ = iCells.Aggregate((i1,
                i2) =>
                (Math.Abs(i1.GameObject.transform.position.z) > Math.Abs(i2.GameObject.transform.position.z) ? i1 : i2)).GameObject.transform.position.z;

            var x = Math.Abs((int) (maxX / CellSize)) + 1;
            var z = Math.Abs((int) (maxZ / CellSize)) + 1;
            Debug.Log("x" + x +  ": z" + z);
            cells = new ICell[x,z];

            rects = new Rect[x,z];
            for (int i = 0; i < rects.GetLength(0); i++)
            {
                for (int j = 0; j < rects.GetLength(1); j++)
                {
                    var pos = GetPosition(i, j);
                    rects[i,j] = new Rect(pos.x,pos.z, CellSize, CellSize);
               }
            }
            foreach (ICell cell in iCells)
            {
                for (int i = 0; i < rects.GetLength(0); i++)
                {
                    for (int j = 0; j < rects.GetLength(1); j++)
                    {
                      Vector2 pos = new Vector2(cell.GameObject.transform.position.x, cell.GameObject.transform.position.z);
                        if (rects[i, j].Contains(pos))
                        {
                            cells[i, j] = cell;
                            cell.GridX = i;
                            cell.GridY = j;
                            break;;
                        }
                   }
                }
            }
        }
        Debug.Log(cells);
    }

    public void ReadLevel(string levelName)
    {
        TextAsset level = Resources.Load(levelName) as TextAsset;
        if (!level)
        {
            throw new Exception("Level file not Found");
        }
        //create parent
        CellParent = new GameObject("Boxes");


        var lines = level.text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        Height = lines.Length;
        Width = lines[0].Length;
        char[,] mapInChar = new char[Width,Height];
        for(int line = 0; line < Height; line++)
        {
            var l = lines[line];
            if (l.Length != Width)
            {
                throw new Exception("Invalid map, Length Must be constance");
            }
            for (int c = 0; c < Width; c++)
            {
                char ch = l[c];
                mapInChar[c, line] = ch;
            }
        }
        InstanceGrid(mapInChar);
    }

    private void InstanceGrid(char[,] mapData)
    {
        cells = new ICell[Width,Height];
        for (int i = 0; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                switch (mapData[i,j])
                {
                    case 'M':
                        cells[i, j] = InitalCell(MoveableCell, i, j);
                        break;
                    case 'F':
                        cells[i, j] = InitalCell(FixedCell, i, j);
                        break;
                    case 'C':
                        cells[i, j] = InitalCell(ChainPushableCell, i, j);
                        break;
                    case 'P':
                        cells[i, j] = InitalCell(PlayerCell, i, j);
                        break;
                    case 'X':
                        cells[i, j] = InitalCell(ExitCell, i, j);
                        break;
				    case 'R':
					    cells[i, j] = InitalCell(ResetCel, i, j);
					    break;
				default:
					cells[i, j] = InitalCell(EmptyCell, i, j);
                        break;
                }

                
           }
        }
    }


    private ICell InitalCell(GameObject gameObject, int x, int y)
    {
        GameObject instance = Instantiate(gameObject);
        instance.name += "_" + CellId++;
        Component component = instance.GetComponent(typeof(ICell));
        ICell cell = component as ICell;
        if (cell == null)
        {
            throw new Exception("Cell Create Faild, Objective do not have ICell component");
        }
        cell.GridX = x;
        cell.GridY = y;
        instance.transform.position = GetPosition(x, y);
        instance.transform.SetParent(CellParent.transform);

        return cell;
    }
	// Update is called once per frame
	void Update () {
	}

    private Vector3 GetPosition(int x, int y)
    {
        return new Vector3(x * CellSize + x * CellSpacing ,0, y * -CellSize - y * CellSpacing);
    }
    public Vector3 MoveLeft(ICell aCell)
    {
        if (aCell.GridX <= 0)
        {
            //if not moved return same position
            return aCell.GameObject.transform.position;
        }
        ICell bCell = cells[aCell.GridX -1, aCell.GridY];

        return Move(aCell, bCell);
    }
    
    public Vector3 MoveRight(ICell aCell)
    {
        if (aCell.GridX >= cells.GetLength(0) - 1)
        {
            //if not moved return same position
            return aCell.GameObject.transform.position;
        }
        ICell bCell = cells[aCell.GridX + 1, aCell.GridY];
        return Move(aCell, bCell);
    }

    private Vector3 Move(ICell aCell,
        ICell bCell)
    {
        if (bCell is MoveOnceCell && !(aCell is MoveOnceCell))
        {
            var other = bCell as MoveOnceCell;
            int oldX = aCell.GridX;
            int oldY = aCell.GridY;
            int newX = bCell.GridX;
            int newY = bCell.GridY;
            bool isMoved = MoveMoveableCells(other, oldX, oldY, newX, newY);
            if (isMoved)
            {
                Destroy(cells[newX, newY].GameObject);
                cells[newX, newY] = aCell;
                aCell.GridX = newX;
                aCell.GridY = newY;
                cells[oldX, oldY] = InitalCell(EmptyCell, oldX, oldY);
                var targetPosition = GetPosition(newX, newY);
                return targetPosition;
            }
        }
        if (bCell is ChainPushableCell)
        {
            ChainPushableCell pCell = bCell as ChainPushableCell;
            int oldX = aCell.GridX;
            int oldY = aCell.GridY;
            int newX = bCell.GridX;
            int newY = bCell.GridY;

            var isMoved = MoveMoveableCells(pCell, oldX, oldY, newX, newY);
            if (isMoved) {
                Destroy(cells[newX, newY].GameObject);
                cells[newX, newY] = aCell;
                aCell.GridX = newX;
                aCell.GridY = newY;
                cells[oldX, oldY] = InitalCell(EmptyCell, oldX, oldY);
                var targetPosition = GetPosition(newX, newY);
                return targetPosition;
            }
        }
        if (bCell is IReplacableCell)
        {
            int oldX = aCell.GridX;
            int oldY = aCell.GridY;
            int newX = bCell.GridX;
            int newY = bCell.GridY;

            Destroy(bCell.GameObject);
            cells[newX, newY] = aCell;
            aCell.GridX = newX;
            aCell.GridY = newY;
            cells[oldX, oldY] = InitalCell(EmptyCell, oldX, oldY);
            var targetPosition = GetPosition(newX, newY);
            return targetPosition;
        }
        if(bCell is IExitCell || bCell is IResetCell)
        {
            int oldX = aCell.GridX;
            int oldY = aCell.GridY;
            int newX = bCell.GridX;
            int newY = bCell.GridY;
            cells[newX, newY] = aCell;
            aCell.GridX = newX;
            aCell.GridY = newY;
            cells[oldX, oldY] = InitalCell(EmptyCell, oldX, oldY);
            var targetPosition = GetPosition(newX, newY);
            return targetPosition;
        }

        return aCell.GameObject.transform.position;
    }

    //return true if the cell is acutall moved
    private static bool MoveMoveableCells(MovableCell pCell,
                                            int oldX,
                                            int oldY,
                                            int newX,
                                            int newY
)
    {
        bool isMoved = false;
        if (newX < oldX)
        {
            isMoved = pCell.MoveLeft();
        }
        else if (newX > oldX)
        {
            isMoved = pCell.MoveRight();
        }
        if (newY < oldY)
        {
            isMoved = pCell.MoveUp();
        }
        else if (newY > oldY)
        {
            isMoved = pCell.MoveDown();
        }
        return isMoved;
    }

    public Vector3 MoveUp(ICell aCell)
    {
        if (aCell.GridY <= 0)
        {
            //if not moved return same position
            return aCell.GameObject.transform.position;
        }
        ICell bCell = cells[aCell.GridX, aCell.GridY -1];
        return Move(aCell, bCell);
    }

    public Vector3 MoveDown(ICell aCell)
    {
        if (aCell.GridY >= cells.GetLength(1) - 1)
        {
            //if not moved return same position
            return aCell.GameObject.transform.position;
        }
        ICell bCell = cells[aCell.GridX, aCell.GridY + 1];
        return Move(aCell, bCell);
    }

}
public interface ICell
{
    int GridX { set; get; }
    int GridY { set; get; }
    GameObject GameObject { get; }
}

public interface IPlayerCell : ICell
{
    
}
public interface IReplacableCell : ICell
{

}

public interface IExitCell : ICell
{
    
}
public interface IResetCell : ICell{

}

public interface IChainPushableCell : ICell
{
    bool MoveLeft();
    bool MoveRight();
    bool MoveUp();
    bool MoveDown();
}


public class AutoIncreaseList<T> : List<T> where T : new()
{
    public T this[int key]
    {
        get { return base[key]; }
        set
        {
            if (key >= base.Count)
            {
                var extra = key - base.Count;
                for (int i = 0; i < extra; i++)
                {
                    base.Add(new T());
                }
            }
        }
    }

    public AutoIncreaseList()
    {
        
    }
    public AutoIncreaseList(int size)
    {
        base.AddRange(Enumerable.Repeat(default(T), size));
    } 
}

[System.Serializable]
 public class Serialiseable2DArray : IEnumerable
  {

    [SerializeField]
    public GameObject[] _items;
    private Size _size;
 
    public Serialiseable2DArray()
    { }
 
    public Serialiseable2DArray(int width, int height)
      : this(new Size(width, height))
    { }

    public Serialiseable2DArray(Size size)
      : this()
    {
      this.Size = size;
    }

    public IEnumerator<GameObject> GetEnumerator()
    {
        foreach (GameObject item in _items)
        yield return item;
    }
 
    public int GetItemIndex(int x, int y)
    {
      if (x < 0 || x >= this.Size.Width)
        throw new IndexOutOfRangeException("X is out of range");
      else if (y < 0 || y >= this.Size.Height)
        throw new IndexOutOfRangeException("Y is out of range");
 
      return y * this.Size.Width + x;
    }
 
    public int GetItemIndex(Point point)
    {
      return this.GetItemIndex(point.X, point.Y);
    }
 
    public Point GetItemLocation(int index)
    {
      Point point;
 
      if (index < 0 || index >= _items.Length)
        throw new IndexOutOfRangeException("Index is out of range");
 
      point = new Point();
      point.Y = index / this.Size.Width;
      point.X = index - (point.Y * this.Size.Height);
 
      return point;
    }
 
    public int Count
    { get { return _items.Length; } }
 
    public Size Size
    {
      get { return _size; }
      set
      {
        _size = value;
        _items = new GameObject[_size.Width * _size.Height];
      }
    }

    public GameObject this[Point location]
    {
      get { return this[location.X, location.Y]; }
      set { this[location.X, location.Y] = value; }
    }

    public GameObject this[int x, int y]
    {
      get { return this[this.GetItemIndex(x, y)]; }
      set { this[this.GetItemIndex(x, y)] = value; }
    }

    public GameObject this[int index]
    {
      get { return _items[index]; }
      set { _items[index] = value; }
    }
 
    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }
  }
[System.Serializable]
public class Size
{
    public int Width;
    public int Height;

    public Size(int w,
        int h)
    {
        this.Width = w;
        this.Height = h;
    }
}

public struct Point
{
    public int X;
    public int Y;
}