using System;
using System.Collections.Generic;
using PriorityQueues; 
using UnityEngine;


public class ProceduralBoard : Board
{
    // inherits all properties of Board, below are two new properties.
    private bool[,] taken; // used to generate board and track free spaces
    private List<(int,int)> targetIdx;
    private int[,,] shortestPathTrees;
    private int nPenguins;

    // procedurally generate a 16x16, 4-penguin board
    public ProceduralBoard(int nPenguins_)
    {
        RowCells = ColumnCells = 16;
        Rows = Columns = RowCells * 2 + 1;
        Obstacles = new int[Rows,Columns];
        Penguins = new int[Rows,Columns];
        Targets = new int[Rows,Columns];

        // this will ensure no corners are too close to each other
        nPenguins = nPenguins_;
        taken = new bool[RowCells, ColumnCells];
        targetIdx = new List<(int,int)>();
        shortestPathTrees = new int[nPenguins, RowCells, ColumnCells];

        // create outside edges
        for (int i = 0; i < Columns; i++)
        {
            Obstacles[0,i] = Obstacles[i,0]  = 1; // top left corner
            Obstacles[Rows-1,i] = Obstacles[i,Columns-1] = 1; // bottom right corner
        }
        // create center square
        int start = CellToCoord(7) - 1, end = CellToCoord(9);
        for (int i = start; i < end; i++)
        for (int j = start; j < end; j++)
            Obstacles[i,j] = 1;

        // (and mark them as "taken")
        for (int i = 6; i <= 9; i++) 
        for (int j = 6; j <= 9; j++)
            taken[i,j] = true;

        // fill each quadrant w/ 4 L walls and 2 border walls
        for (int quadY = 0; quadY <= 1; quadY++)
        for (int quadX = 0; quadX <= 1; quadX++)
        {
            addLWalls(quadY, quadX);
            addBorderWalls(quadY, quadX);
        }        
        // now add the penguins
        addPenguins();
        assignTargets();
    }

    private void assignTargets()
    {
        int nTargets = targetIdx.Count;
        // will be sorted according to distance
        int[][] idx = new int[nPenguins][];
        for (int i = 0; i < nPenguins; i++) idx[i] = new int[nTargets];
        // has this target been used yet?
        bool[] allocated = new bool[nTargets];

        for (int p = 0; p < nPenguins; p++)
        {
            int[] dist = new int[nTargets];
            for (int t = 0; t < nTargets; t++)
            {
                idx[p][t] = t;
                int y,x; (y,x) = targetIdx[t];
                dist[t] = shortestPathTrees[p,y,x];
            }
            // sort indices by distance from this penguin to that target
            Array.Sort(dist,idx[p],0,nTargets,null);
        }
        // now 'draft' all targets to the robot which is furthest from it
        for (int p=0,n=0; n < nTargets; p=(p+1)%nPenguins,n++)
        {
            for (int i = nTargets-1; i >= 0; i--)
            {
                int hardest = idx[p][i];
                if (!allocated[hardest])
                {
                    int y,x; (y,x) = targetIdx[hardest];
                    Debug.Log(shortestPathTrees[p,y,x]);
                    int Y = CellToCoord(y), X = CellToCoord(x);
                    Targets[Y,X] = p+1;
                    allocated[hardest] = true;
                    break;
                }
            }
        }
    }

    private void addPenguins()
    {
        // function to determine if a given cell is a valid penguin placement
        bool penguinUtil(int i, int j)
        {
            int I = CellToCoord(i), J = CellToCoord(j);
            return Targets[I, J] == 0 && Penguins[I, J] == 0;
        }

        for (int i = 0; i < nPenguins; i++)
        {
            var cell = randMatchIdx(0,RowCells,0,ColumnCells,penguinUtil);
            if (cell == null) throw new Exception("No space for penguin "+i);
            int y,x; (y,x) = cell ?? (0,0);
            // now place the penguin
            int Y = CellToCoord(y), X = CellToCoord(x);
            Penguins[Y, X] = i + 1;

            // now calculate shortest path tree and save it for later
            var spt = calculateSPT(y,x);
            for (int j = 0; j < RowCells; j++)
                for (int k = 0; k < ColumnCells; k++)
                    shortestPathTrees[i,j,k] = spt[j,k];
        }
    }
    // uses dijkstra's algo to generate shortest path tree
    // used to enable optimal target allocation between penguins
    private int[,] calculateSPT(int y, int x)
    {
        bool[,] visited = new bool[RowCells, ColumnCells];
        int[,] dist = new int[RowCells, ColumnCells];
        // set initial distance to infinity
        for (int i = 0; i < RowCells; i++)
            for (int j = 0; j < ColumnCells; j++)
                dist[i,j] = Int32.MaxValue;
        // set source distance to zero
        dist[y,x] = 0;
        
        // tuple is (y,x, dy,dx), priority is number of moves
        var q = new BinaryHeap<(int,int,int,int), int>(PriorityQueueType.Minimum);
        q.Enqueue((y,x, 0,0), dist[y,x]);
        while (q.Count > 0)
        {
            int i=0,j=0, dy_=0, dx_=0;
            bool failed = false;
            do 
            { 
                try { (i,j, dy_, dx_) = q.Dequeue(); }
                catch { failed = true; break; }
            } while (!failed && visited[i,j]); 
            if (failed) break;

            visited[i,j] = true;

            // enqueue neighbors distances
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    int Y = CellToCoord(i)+dy, X = CellToCoord(j)+dx;
                    // first make sure it's a valid transition
                    if (Math.Abs(dy) + Math.Abs(dx) != 1 || // invalid direction
                        !CellIsInBounds(i+dy, j+dx) || // out of bounds
                        visited[i+dy,j+dx] || // already been here
                        Obstacles[Y, X] != 0 // wall in the way
                        // || Penguins[Y, X] != 0 // penguin in the way (do we want this?)
                    ){
                        continue; 
                    }
                    //Debug.Log((Y,X));
                    // calculate cost of moving in this direction 
                    int cost = dist[i,j] + (dy==dy_ && dx==dx_ ? 0 : 1);
                    if (cost < dist[i+dy, j+dx])
                    {
                        q.Enqueue((i+dy,j+dx,dy,dx), cost);
                        dist[i+dy, j+dx] = cost;
                    }
                }
            }
        }
        return dist;
    }
    // this utility function returns a random x,y from a rectangular
    // area of x,y's that fit some criteria 
    private (int,int)? randMatchIdx(
        int yTop, int yBottom, int xLeft, int xRight, // bounds
        Func<int, int, bool> doesMatch // predicate to match
    ){
        System.Random rnd = new System.Random(); 
        // store the indices of matches in the order we find them
        List<(int,int)> matches = new List<(int,int)>();
        for (int i = yTop; i < yBottom; i++)
            for (int j = xLeft; j < xRight; j++)
                if (doesMatch(i, j)) matches.Add((i,j));

        // we now have all matches from that region; pick a random one.
        if (matches.Count == 0) return null;
        return matches[rnd.Next(matches.Count)]; 
    }

    private void addBorderWalls(int quadY, int quadX)
    {
        int inwardY = quadY == 0 ? 1 : -1; 
        int inwardX = quadX == 0 ? 1 : -1; 
        // utility functions for determining if a wall can be placed at a cell
        bool horiBorderUtil(int i, int j)
        {
            int I = CellToCoord(i), J = CellToCoord(j);
            for (int k = 0; k < 4; k++)
                if (Obstacles[I+inwardY*k, J-1] != 0) return false;
            return !taken[i,j];
        }
        bool vertBorderUtil(int i, int j)
        {
            int I = CellToCoord(i), J = CellToCoord(j);
            for (int k = 0; k < 4; k++)
                if (Obstacles[I-1, J+inwardX*k] != 0) return false;
            return !taken[i,j];
        }

        int y, x, Y, X;

        // left and right bounds for placing vert walls on horizontal boundary
        int xLeft = quadX == 0 ? 2 : ColumnCells/2 + 1;
        int xRight = xLeft + ColumnCells / 2 - 3;
        int yTop = quadY == 0 ? 2 : RowCells/2 + 1;
        int yBottom = yTop + RowCells / 2 - 3;

        y = quadY * (RowCells - 1);
        var cell = randMatchIdx(y, y+1, xLeft, xRight, horiBorderUtil);
        if (cell == null) throw new Exception("No space for wall");
        (y,x) = cell ?? (0,0); // y should go unchanged

        // we've now got our random cell; convert to real indices and place the wall.
        Y = CellToCoord(y); X = CellToCoord(x) - 1;
        Obstacles[Y, X] = Obstacles[Y+inwardY, X] = 1;

        // do the same for vertical boundaries now
        x = quadX * (ColumnCells - 1);
        cell = randMatchIdx(yTop, yBottom, x, x+1, vertBorderUtil);
        if (cell == null) throw new Exception("No space for wall");
        (y,x) = cell ?? (0,0); // y should go unchanged

        // we've now got our random cell; convert to real indices and place the wall.
        Y = CellToCoord(y) - 1;
        X = CellToCoord(x);
        Obstacles[Y, X] = Obstacles[Y, X+inwardX] = 1;
    }

    private void addLWalls(int quadY, int quadX)
    {
        // boundaries of quadrant
        int qTop = quadY == 0 ? 1 : RowCells/2;
        int qBottom = qTop + RowCells/2-2;
        int qLeft = quadX == 0 ? 1 : ColumnCells/2;
        int qRight = qLeft + ColumnCells/2-2;
        // iterate through relative corner coords
        for (int cY = -1; cY <= 1; cY += 2)
        for (int cX = -1; cX <= 1; cX += 2)
        {
            // this function determines if a particular CELL is
            // a suitable location for the curent L wall
            bool validCorner(int i, int j)
            {
                if (taken[i,j]) return false;
                int I = CellToCoord(i), J = CellToCoord(j);
                // (sorry for these cryptic ass loops)
                // check for "collisions" with nearby L walls 

                // ( overshoot the end of the vertical wall & clamp to board size
                // ; while you haven't overshot in the opposite direction...
                // ; move 2 in the opposite direction
                for ( int k = Math.Max(Math.Min( I-cY*4, Rows-2 ), 1)
                    ; k != I+6*cY && 0 <= k && k < Columns
                    ; k += 2*cY 
                ){
                    if (Obstacles[k, J+cX] != 0) return false;
                }
                // same structure as loop above, just changed for horizontal wall
                for ( int k = Math.Max(Math.Min( J-cX*4, Rows-2 ), 1)
                    ; k != J+6*cX && 0 <= k && k < Columns
                    ; k += 2*cX 
                ){
                    if (Obstacles[I+cY, k] != 0) return false;
                }

                return true;
            }
            // pick a random spot in this quadrant
            var cell = randMatchIdx(qTop,qBottom,qLeft,qRight, validCorner);
            if (cell == null) throw new Exception("No space for corner!");
            // we know cell isn't null but null coalescing is necessary for cast
            int y,x; (y,x) = cell ?? (0,0);
            
            // place the corner
            int Y = CellToCoord(y), X = CellToCoord(x);
            for (int i = 0; i < 3; i++)
                Obstacles[Y + cY, X + cX - i*cX] = 
                    Obstacles[Y + cY - i*cY, X + cX] = 1;

            // note this so we can put a target here eventually
            targetIdx.Add((y,x));
            // also mark it in the Targets array with a temp '-1' for quick lookup
            Targets[Y,X] = -1;
            // update the taken array
            // first the whole row and column within this quadrant
            for (int i=qTop; i<qTop+RowCells/2-1; i++) taken[i,x] = true;
            for (int j=qLeft; j<qLeft+ColumnCells/2-1; j++) taken[y,j] = true;
            // now the bordering cells
            for (int i = 0; i < 3; i++)
                taken[y-(1-i),x-1] = taken[y-(1-i),x+1] = 
                taken[y-1,x-(1-i)] = taken[y+1,x-(1-i)] = true;
        }
    }
}
