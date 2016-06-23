using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(GridLayoutGroup))]
public class DynamicGridLayout : MonoBehaviour
{
    private RectTransform m_GridRectTransform = null;
    [SerializeField]
    private GridLayoutGroup m_GridLayoutGroup = null;

    void Awake()
    {
        m_GridRectTransform = this.GetComponent<RectTransform>();
        m_GridLayoutGroup = this.GetComponent<GridLayoutGroup>();
    }

    void Start()
    {
        ResizeCell();
    }
    
    void OnRectTransformDimensionsChange()
    {
        ResizeCell();
    }

    private void ResizeCell()
    {
        if (!m_GridRectTransform || !m_GridLayoutGroup) { return; }

        Vector2 cellSize = m_GridLayoutGroup.cellSize;
	    switch (m_GridLayoutGroup.constraint)
        {
            case GridLayoutGroup.Constraint.FixedColumnCount:
                cellSize.x = m_GridRectTransform.rect.width / m_GridLayoutGroup.constraintCount;
                m_GridLayoutGroup.cellSize = cellSize;
                break;
            case GridLayoutGroup.Constraint.FixedRowCount:
                cellSize.y = m_GridRectTransform.rect.height / m_GridLayoutGroup.constraintCount;
                m_GridLayoutGroup.cellSize = cellSize;
                break;
            case GridLayoutGroup.Constraint.Flexible:
                break;
        }
	}
}
