using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineGenerator : MonoBehaviour
{
    public GameObject linePrefab;

    Line activeLine;
    List<Line> allLines = new List<Line>();

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject newLine = Instantiate(linePrefab);
            activeLine = newLine.GetComponent<Line>();
        }

        if (Input.GetMouseButtonUp(0) && activeLine != null)
        {
            allLines.Add(activeLine);
            activeLine = null;
        }

        if(activeLine != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            activeLine.UpdateLine(mousePos);
        }

        //Undo last line
        if (Input.GetMouseButtonDown(1))
        {
            if(allLines.Count == 0) {return;}
            Line last = allLines[allLines.Count-1];
            allLines.Remove(last);
            Destroy(last.gameObject);
        }
    }
}
