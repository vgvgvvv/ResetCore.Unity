using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tree<T> {

    public Tree(){ }
    public Tree(T node)
    {
        this.node = node;
    }

    public T node { get; private set; }
    private List<Tree<T>> children = new List<Tree<T>>();

    public void AddChild(T child)
    {
        Tree<T> node = new Tree<T>(child);
        children.Add(node);
    }

    public void AddChild(Tree<T> child){
        children.Add(child);
    }

    public void RemoveChild(T child)
    {
        foreach (Tree<T> node in children)
        {
            if (node.node.Equals(child))
            {
                children.Remove(node);
                break;
            }
        }
    }

    public void RemoveChild(Tree<T> child)
    {
        children.Remove(child);
    }
	
}
