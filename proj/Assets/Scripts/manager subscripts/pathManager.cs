using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//used the unity wiki as reference as well as project 2 code
public class pathManager : MonoBehaviour {

	public gameManager gm;

	private List<Vector2> currentPath;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	List<Vector2> connections(Vector2 pos)
	{
		List<Vector2> res = new List<Vector2>();
		Vector2[] possibleMoves = new Vector2[4];
		possibleMoves[0] = new Vector2(pos.x-1,pos.y);
		possibleMoves[1] = new Vector2(pos.x+1,pos.y);
		possibleMoves[2] = new Vector2(pos.x,pos.y-1);
		possibleMoves[3] = new Vector2(pos.x,pos.y+1);

		foreach(Vector2 move in possibleMoves) {
			if(move.x<gm.gc.gridWidth && move.y<gm.gc.gridHeight && move.x >= 0 && move.y >= 0) {
				if(gm.traversible[(int)move.x,(int)move.y])
					res.Add(new Vector2(move.x,move.y));
			}
		}
		return res;
	}

	float estimate(Vector2 startPos,Vector2 endPos)
	{
		return (gm.totalInfluence/gm.influence.Length)*(Mathf.Abs(startPos.x - endPos.x) + Mathf.Abs(startPos.y - endPos.y));
	}

	Vector2 findLowest(List<Vector2> list, Dictionary<Vector2,float> scores)
	{
		int index = 0;
		float min = float.MaxValue;

		for(int i = 0; i < list.Count; i++)
		{
			if(scores[list[i]] < min)
			{
				index = i;
				min = scores[list[i]];
			}
		}
		return list[index];
	}

	public ArrayList updatePath(Vector2 startPos, Vector2 endPos)
	{
		List<Vector2> closed = new List<Vector2>();
		List<Vector2> open = new List<Vector2>();
		open.Add(startPos);

		Dictionary<Vector2,Vector2> came_from = new Dictionary<Vector2,Vector2>();
		Dictionary<Vector2,float> g = new Dictionary<Vector2,float>();
		g[startPos] = 0.0f;
		Dictionary<Vector2,float> h = new Dictionary<Vector2,float>();
		h[startPos] = estimate(startPos,endPos);
		Dictionary<Vector2,float> f = new Dictionary<Vector2,float>();
		f[startPos] = h[startPos];

		while(open.Count != 0)
		{
			Vector2 current = findLowest(open,f);
			if(current.x == endPos.x && current.y == endPos.y)
			{
				//return reconstructed path
				ArrayList result = new ArrayList();
				while(came_from.ContainsKey(current))
				{
					result.Insert(0,current);
					//Debug.Log(g[current]);
					current = came_from[current];
				}
				result.Insert(0,current);
				return result;
			}
			open.Remove(current);
			closed.Add(current);
			foreach(Vector2 next in connections(current))
			{
				if(!closed.Contains(next))
				{
					float initialScore = g[current] + gm.influence[(int)current.x,(int)current.y];
					bool better = false;
					if(!open.Contains(next))
					{
						open.Add(next);
						better = true;
					}
					else if(initialScore < g[next])
					{
						better = true;
					}
					if(better)
					{
						came_from[next] = current;
						g[next] = initialScore;
						h[next] = estimate(next,endPos);
						f[next] = g[next] + h[next];
					}
				}
			}
		}
		return null;
	}
}
