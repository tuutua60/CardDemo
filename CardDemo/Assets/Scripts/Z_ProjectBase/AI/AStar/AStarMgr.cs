using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A* 寻路管理器 (已优化)
/// </summary>
public class AStarMgr : BaseManager<AStarMgr>
{
    // 地图的宽高
    private int mapWidth;
    private int mapHeight;
    // 地图所有节点的二维数组
    public AStarNode[,] nodes;
    
    // 开放列表（待考察的节点）
    private List<AStarNode> openList = new List<AStarNode>();
    // 封闭列表（已考察过的节点），使用HashSet提高查询效率
    private HashSet<AStarNode> closeList = new HashSet<AStarNode>();

    /// <summary>
    /// 初始化地图信息
    /// </summary>
    public void InitMapInfo(int w, int h)
    {
        this.mapWidth = w;
        this.mapHeight = h;
        
        nodes = new AStarNode[w, h];
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                // 随机生成障碍物
                E_Node_Type t = Random.Range(0, 100) < 20 ? E_Node_Type.Obstacle : E_Node_Type.Walkable;
                nodes[i, j] = new AStarNode(i, j, t);
            }
        }
    }

    /// <summary>
    /// 查找路径
    /// </summary>
    /// <param name="startPos">起点坐标</param>
    /// <param name="endPos">终点坐标</param>
    /// <returns>包含路径节点的列表，如果找不到则返回null</returns>
    public List<AStarNode> FindPath(Vector2 startPos, Vector2 endPos)
    {
        // 1. 安全性判断，确保坐标在地图范围内
        if (startPos.x < 0 || startPos.x >= mapWidth ||
            startPos.y < 0 || startPos.y >= mapHeight ||
            endPos.x < 0 || endPos.x >= mapWidth ||
            endPos.y < 0 || endPos.y >= mapHeight)
        {
            Debug.LogError("寻路失败：起点或终点在地图范围外。");
            return null;
        }

        AStarNode startNode = nodes[(int)startPos.x, (int)startPos.y];
        AStarNode endNode = nodes[(int)endPos.x, (int)endPos.y];

        // 2. 判断起点和终点是否为障碍物
        if (startNode.type == E_Node_Type.Obstacle || endNode.type == E_Node_Type.Obstacle)
        {
            Debug.LogError("寻路失败：起点或终点是障碍物。");
            return null;
        }

        // 3. 重置所有节点状态，清空列表，为本次寻路做准备
        ResetNodes();
        closeList.Clear();
        openList.Clear();

        // 4. 将起点放入开放列表
        startNode.father = null;
        startNode.g = 0;
        startNode.h = GetH(startNode, endNode);
        startNode.f = startNode.g + startNode.h;
        openList.Add(startNode);

        // 5. 主循环，直到开放列表为空（无路可走）
        while (openList.Count > 0)
        {
            // 5.1 从开放列表中找到F值最小的节点
            AStarNode currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].f < currentNode.f || (openList[i].f == currentNode.f && openList[i].h < currentNode.h))
                {
                    currentNode = openList[i];
                }
            }

            // 5.2 将当前节点从开放列表移到封闭列表
            openList.Remove(currentNode);
            closeList.Add(currentNode);

            // 5.3 如果当前节点是终点，则寻路成功
            if (currentNode == endNode)
            {
                return GeneratePath(endNode);
            }

            // 5.4 遍历当前节点的邻居
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    // 忽略自身
                    if (i == 0 && j == 0) continue;

                    int neighborX = currentNode.x + i;
                    int neighborY = currentNode.y + j;

                    // 边界检查
                    if (neighborX < 0 || neighborX >= mapWidth || neighborY < 0 || neighborY >= mapHeight) continue;

                    AStarNode neighbor = nodes[neighborX, neighborY];

                    // 如果是障碍物或已在封闭列表中，则跳过
                    if (neighbor.type == E_Node_Type.Obstacle || closeList.Contains(neighbor)) continue;
                    
                    // 计算G值（对角线消耗更大）
                    float cost = (i != 0 && j != 0) ? 1.4f : 1f;
                    float newG = currentNode.g + cost;

                    // 如果新路径更优，或者邻居不在开放列表中
                    if (newG < neighbor.g || !openList.Contains(neighbor))
                    {
                        neighbor.g = newG;
                        neighbor.h = GetH(neighbor, endNode);
                        neighbor.f = neighbor.g + neighbor.h;
                        neighbor.father = currentNode;

                        if (!openList.Contains(neighbor))
                        {
                            openList.Add(neighbor);
                        }
                    }
                }
            }
        }

        // 开放列表为空，说明无路可达
        Debug.LogWarning("寻路失败：找不到路径。");
        return null;
    }

    /// <summary>
    /// 使用曼哈顿距离计算H值
    /// </summary>
    private float GetH(AStarNode from, AStarNode to)
    {
        return Mathf.Abs(to.x - from.x) + Mathf.Abs(to.y - from.y);
    }

    /// <summary>
    /// 从终点回溯生成路径
    /// </summary>
    private List<AStarNode> GeneratePath(AStarNode endNode)
    {
        List<AStarNode> path = new List<AStarNode>();
        AStarNode node = endNode;
        while (node != null)
        {
            path.Add(node);
            node = node.father;
        }
        path.Reverse();
        return path;
    }

    /// <summary>
    /// 重置所有节点的数据，为下一次寻路做准备
    /// </summary>
    private void ResetNodes()
    {
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                nodes[i, j].f = 0;
                nodes[i, j].g = float.MaxValue; // G值设为无穷大，方便比较
                nodes[i, j].h = 0;
                nodes[i, j].father = null;
            }
        }
    }
}