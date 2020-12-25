using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class main : MonoBehaviour
{
    public GameObject[] tiles;
    public GameObject point;
    public GameObject fill_button;

    //array holding the tiles that point to the dots in the tilted square
    //0 = none
    //1 = up
    //2 = left
    //3 = down
    //4 = right
    //5 = up (colored)
    //6 = left (colored)
    //7 = down (colored)
    //8 = right (colored)
    int[,,] points;

    int[,] dir =
    {
        { 1, 0 },
        { 0, 0 },
        { 0, 1 },
        { 1, 1 }
    };

    int iteration = 2;
    public bool use_colors = false;
    public bool auto_fill = false;
    List<Vector2Int> empty = new List<Vector2Int>();


    bool rand() { return Random.value >= 0.5; }

    public void fill()
    {
        for (int i = 0; i < empty.Count; i++)
        {
            if (rand())
            {
                if (points[empty[i].y, empty[i].x + 1, 0] == 0)
                {
                    points[empty[i].y, empty[i].x + 1, 0] = 1;
                } else
                {
                    points[empty[i].y, empty[i].x + 1, 1] = 1;
                }

                if (points[empty[i].y + 1, empty[i].x, 0] == 0)
                {
                    points[empty[i].y + 1, empty[i].x, 0] = 3;
                }
                else
                {
                    points[empty[i].y + 1, empty[i].x, 1] = 3;
                }
            }
            else
            {
                if (points[empty[i].y, empty[i].x, 0] == 0)
                {
                    points[empty[i].y, empty[i].x, 0] = 2;
                }
                else
                {
                    points[empty[i].y, empty[i].x, 1] = 2;
                }

                if (points[empty[i].y + 1, empty[i].x + 1, 0] == 0)
                {
                    points[empty[i].y + 1, empty[i].x + 1, 0] = 4;
                }
                else
                {
                    points[empty[i].y + 1, empty[i].x + 1, 1] = 4;
                }
            }
        }
        empty = new List<Vector2Int>();
    }

    public void iterate() {
        gameObject.transform.position = gameObject.transform.position + new Vector3(0, 2, 0);
        iteration++;
        int[,,] n_points = new int[iteration, iteration, 2];
        for (int y = 0; y < iteration - 1; y++) {
            for (int x = 0; x < iteration - 1; x++) {
                if (!((points[y, x, 0] == 2 && points[y, x, 1] == 4) ||
                    (points[y, x, 0] == 4 && points[y, x, 1] == 2) ||
                    (points[y, x, 0] == 1 && points[y, x, 1] == 3) ||
                    (points[y, x, 0] == 3 && points[y, x, 1] == 1)))
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (points[y, x, i] != 0)
                        {
                            int tx = x + dir[points[y, x, i] - 1, 0];
                            int ty = y + dir[points[y, x, i] - 1, 1];
                            if (n_points[ty, tx, 0] == 0)
                            {
                                n_points[ty, tx, 0] = points[y, x, i];
                            }
                            else
                            {
                                n_points[ty, tx, 1] = points[y, x, i];
                            }
                        }
                    }
                }
            }
        }

        points = new int[iteration, iteration, 2];
        for (int y = 0; y < points.GetLength(0); y++)
        {
            for (int x = 0; x < points.GetLength(1); x++)
            {
                points[y, x, 0] = n_points[y, x, 0];
                points[y, x, 1] = n_points[y, x, 1];
            }
        }


        empty = new List<Vector2Int>();
        for (int y = 0; y < points.GetLength(0) - 1; y++)
        {
            for (int x = 0; x < points.GetLength(1) - 1; x++)
            {
                bool btemp = false;
                for (int i = 0; i < empty.Count; i++)
                {
                    if (Mathf.Abs(empty[i].x - x) + Mathf.Abs(empty[i].y - y) == 1)
                    {
                        btemp = true;
                        break;
                    }
                }

                if (btemp)
                {
                    continue;
                }

                if (points[y, x, 0] != 0 && points[y, x, 0] != 4)
                {
                    continue;
                }

                if (points[y, x + 1, 0] != 0 && points[y, x + 1, 0] != 3)
                {
                    continue;
                }

                if (points[y + 1, x + 1, 0] != 0 && points[y + 1, x + 1, 0] != 2)
                {
                    continue;
                }

                if (points[y + 1, x, 0] != 0 && points[y + 1, x, 0] != 1)
                {
                    continue;
                }

                empty.Add(new Vector2Int(x, y));
            }
        }
        if (auto_fill)
        {
            fill();
        }
    }

    void redraw_tiles()
    {
        var clones = GameObject.FindGameObjectsWithTag("clone");
        foreach (var clone in clones) {
            Destroy(clone);
        }

        float t = (points.GetLength(0) - 1) / 2f;
        if (use_colors)
        {
            for (int y = 0; y < points.GetLength(0); y++)
            {
                for (int x = 0; x < points.GetLength(1); x++)
                {
                    int ind = points[y, x, 0] + 4;

                    if (ind != 4)
                    {
                        Instantiate(tiles[ind], new Vector3(x + y - 2 * t, 0, x - y), tiles[ind].transform.rotation);
                    }
                    ind = points[y,x,1] + 4;
                    if (ind != 4)
                    {
                        Instantiate(tiles[ind], new Vector3(x + y - 2 * t, 0, x - y), tiles[ind].transform.rotation);
                    }
                }
            }
        } else
        {
            for (int y = 0; y < points.GetLength(0); y++)
            {
                for (int x = 0; x < points.GetLength(1); x++)
                {
                    int ind = points[y,x,0];
                    Instantiate(point, new Vector3(x + y - 2 * t, 0, x - y), point.transform.rotation);
                    if (ind != 0)
                    {
                        Instantiate(tiles[ind], new Vector3(x + y - 2 * t, 0, x - y), tiles[ind].transform.rotation);
                    }
                    ind = points[y,x,1];
                    if (ind != 0)
                    {
                        Instantiate(tiles[ind], new Vector3(x + y - 2 * t, 0, x - y), tiles[ind].transform.rotation);
                    }
                }
            }
        }
        for (int i = 0; i < empty.Count; i++)
        {
            Instantiate(tiles[0], new Vector3(1f + empty[i].x + empty[i].y - 2 * t, 0, empty[i].x - empty[i].y), tiles[0].transform.rotation);
        }
    }

    void Start()
    {
        if (auto_fill)
        {
            fill_button.SetActive(false);
        }
        if (rand())
        {
            points = new int[,,]
            {
                { { 0, 0 }, { 1, 0 } },
                { { 3, 0 }, { 0, 0 } }
            };
        } else
        {
            points = new int[,,]
            {
                { { 2, 0 }, { 0, 0 } },
                { { 0, 0 }, { 4, 0 } }
            };
        }

    }

    
    void Update()
    {
        redraw_tiles();
        
    }
    void print_points()
    {
        string s = "";
        for (int i = 0; i < points.GetLength(0); i++)
        {
            for (int j = 0; j < points.GetLength(0); j++)
            {
                s += points[i, j, 0] + " " + points[i, j, 1] + "     ";
            }
            s += "\n";
        }
        print(s);
    }
}
