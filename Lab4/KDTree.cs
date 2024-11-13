using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{

    internal class KDTree
    {
        public class Node
        {
            public double[] Point { get; set; }
            public Node Left { get; set; }
            public Node Right { get; set; }

            public Node(double[] point)
            {
                Point = point;
            }
        }

        public Node root;
        private int k;

        public KDTree(int dimensions)
        {
            k = dimensions;
        }

        public void Insert(double[] point) => root = InsertRec(root, point, 0);

        private Node InsertRec(Node node, double[] point, int depth)
        {
            if (node == null) return new Node(point);

            int cd = depth % k;
            if (point[cd] < node.Point[cd])
                node.Left = InsertRec(node.Left, point, depth + 1);
            else
                node.Right = InsertRec(node.Right, point, depth + 1);

            return node;
        }

        public Node FindNearest(double[] target) => FindNearestRec(root, target, 0, root);

        private Node FindNearestRec(Node node, double[] target, int depth, Node best)
        {
            if (node == null) return best;

            if (DistanceSquared(node.Point, target) < DistanceSquared(best.Point, target))
                best = node;

            int cd = depth % k;
            Node nextNode = target[cd] < node.Point[cd] ? node.Left : node.Right;
            Node otherNode = nextNode == node.Left ? node.Right : node.Left;

            best = FindNearestRec(nextNode, target, depth + 1, best);

            if (Math.Abs(node.Point[cd] - target[cd]) < DistanceSquared(best.Point, target))
                best = FindNearestRec(otherNode, target, depth + 1, best);

            return best;
        }

        private double DistanceSquared(double[] point1, double[] point2)
        {
            double dist = 0;
            for (int i = 0; i < k; i++)
                dist += Math.Pow(point1[i] - point2[i], 2);
            return dist;
        }

        public void Delete(double[] point)
        {
            root = DeleteRec(root, point, 0);
        }

        private Node DeleteRec(Node node, double[] point, int depth)
        {
            if (node == null) return null;

            int cd = depth % k;
            if (ArePointsEqual(node.Point, point))
            {
                if (node.Right != null)
                {
                    Node minNode = FindMin(node.Right, cd, depth + 1);
                    node.Point = minNode.Point;
                    node.Right = DeleteRec(node.Right, minNode.Point, depth + 1);
                }
                else if (node.Left != null)
                {
                    Node minNode = FindMin(node.Left, cd, depth + 1);
                    node.Point = minNode.Point;
                    node.Right = DeleteRec(node.Left, minNode.Point, depth + 1);
                    node.Left = null;
                }
                else return null;
            }
            else if (point[cd] < node.Point[cd])
                node.Left = DeleteRec(node.Left, point, depth + 1);
            else
                node.Right = DeleteRec(node.Right, point, depth + 1);

            return node;
        }

        private Node FindMin(Node node, int d, int depth)
        {
            if (node == null) return null;

            int cd = depth % k;
            if (cd == d)
            {
                if (node.Left == null) return node;
                return FindMin(node.Left, d, depth + 1);
            }

            Node left = FindMin(node.Left, d, depth + 1);
            Node right = FindMin(node.Right, d, depth + 1);
            Node min = node;

            if (left != null && left.Point[d] < min.Point[d]) min = left;
            if (right != null && right.Point[d] < min.Point[d]) min = right;

            return min;
        }

        private bool ArePointsEqual(double[] point1, double[] point2)
        {
            for (int i = 0; i < k; i++)
                if (point1[i] != point2[i]) return false;
            return true;
        }
        public List<double[]> GetAllPoints()
        {
            List<double[]> points = new List<double[]>();
            CollectPoints(root, points);
            return points;
        }

        private void CollectPoints(Node node, List<double[]> points)
        {
            if (node == null) return;
            points.Add(node.Point);
            CollectPoints(node.Left, points);
            CollectPoints(node.Right, points);
        }
    }

}

