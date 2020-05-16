using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS_MidtermProject {

    // this is actually the avl tree
    class BSTNode {
        public IndexNode data;
        public BSTNode lc, rc;
        public int height = 1;

        public BSTNode() { }

        public BSTNode(IndexNode data) {
            this.data = data;
        }
    }

    public class BST {

        BSTNode root;
        Order order;

        public enum Order {
            preorder,
            inorder,
            postorder
        }

        public BST(List<IndexNode> s) {
            foreach (IndexNode t in s) {
                root = insert(root, t);
            }
        }

        BSTNode rRotate(BSTNode y) {
            if (y == null) return null;

            BSTNode x = y.lc;
            BSTNode z = (x != null) ? x.rc : null;

            if (x != null) x.rc = y;
            y.lc = z;

            if (x != null) x.height = Math.Max(getNodeHeight(x.lc), getNodeHeight(x.rc)) + 1;
            y.height = Math.Max(getNodeHeight(y.lc), getNodeHeight(y.rc)) + 1;

            return x;
        }

        BSTNode lRotate(BSTNode x) {
            if (x == null) return null;

            BSTNode y = x.rc;
            BSTNode z = (y != null) ? y.lc : null;

            if (y != null) y.lc = x;
            x.rc = z;

            x.height = Math.Max(getNodeHeight(x.lc), getNodeHeight(x.rc)) + 1;
            if (y != null) y.height = Math.Max(getNodeHeight(y.lc), getNodeHeight(y.rc)) + 1;

            return y;
        }

        BSTNode insert(BSTNode node, IndexNode t) {
            if (node == null) {
                return (new BSTNode(t));
            }

            string data = t.word;

            if (node.data.word.CompareTo(data) > 0) {
                node.lc = insert(node.lc, t);
            } else {
                node.rc = insert(node.rc, t);
            }

            node.height = Math.Max(getNodeHeight(node.lc), getNodeHeight(node.rc)) + 1;

            int diff = getNodeBalance(node);

            // Left Rotate
            if (diff > 1 && data.CompareTo(node.lc.data.word) < 0) {
                return rRotate(node);
            }

            // Right Rotate
            if (diff < -1 && data.CompareTo(node.rc.data.word) > 0) {
                return lRotate(node);
            }

            // Left Right Rotate
            if (diff > 1 && data.CompareTo(node.lc.data.word) > 0) {
                node.lc = lRotate(node.lc);
                return rRotate(node);
            }

            // Right Left Rotate
            if (diff < -1 && data.CompareTo(node.rc.data.word) < 0) {
                node.rc = rRotate(node.rc);
                return lRotate(node);
            }

            return node;
        }

        public IndexNode search(string word) {
            return search(root, word);
        }

        IndexNode search(BSTNode node, string word) {
            if (node == null) return null;
            if (node.data.word == word) return node.data;
            if (node.data.word.CompareTo(word) > 0) return search(node.lc, word);
            return search(node.rc, word);
        }

        void print(BSTNode head) {
            if (head != null) {
                switch (order) {
                    case Order.preorder:
                        Console.Write(head.data.word + ", ");
                        print(head.lc);
                        print(head.rc);
                        break;
                    case Order.inorder:
                        print(head.lc);
                        Console.Write(head.data.word + ", ");
                        print(head.rc);
                        break;
                    case Order.postorder:
                        print(head.lc);
                        print(head.rc);
                        Console.Write(head.data.word + ", ");
                        break;
                }
            }
        }

        public void print(Order order) {
            this.order = order;
            print(root);
        }

        public int depth() {
            return depth(root);
        }

        int depth(BSTNode t) {
            if (t == null) return 0;
            return Math.Max(depth(t.lc), depth(t.rc)) + 1;
        }

        int getNodeHeight(BSTNode t) {
            if (t == null) return 0;
            return t.height;
        }

        int getNodeBalance(BSTNode t) {
            if (t == null) return 0;
            return (getNodeHeight(t.lc) - getNodeHeight(t.rc));
        }

    }
}
