using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace RedsTools
{
    namespace WPF
    {
        public static class WPFTypes
        {
            public static List<Visual> GetChildrens(Visual iParent, bool iRecursive)
            {
                List<Visual> result = new List<Visual>();

                iParent.Dispatcher.Invoke((Action)(() =>
                {
                    if (iRecursive)
                    {
                        Queue<Visual> toCheck = new Queue<Visual>();

                        toCheck.Enqueue(iParent);

                        while (toCheck.Count > 0)
                        {
                            if (toCheck.Peek() is ScrollViewer)
                            {
                                ScrollViewer scroll = toCheck.Peek() as ScrollViewer;
                                if (scroll.Content != null)
                                {
                                    toCheck.Enqueue(scroll.Content as Visual);
                                    result.Add(scroll.Content as Visual);
                                }
                            }
                            else
                            {
                                for (int j = 0; j < VisualTreeHelper.GetChildrenCount(toCheck.Peek()); j++)
                                {
                                    Visual childVisual = (Visual)VisualTreeHelper.GetChild(toCheck.Peek(), j);

                                    toCheck.Enqueue(childVisual);
                                    result.Add(childVisual);
                                }
                            }

                            toCheck.Dequeue();
                        }
                    }
                    else
                    {
                        if (iParent is ScrollViewer)
                        {
                            ScrollViewer scroll = iParent as ScrollViewer;
                            if (scroll.Content != null)
                            {
                                result.Add(scroll.Content as Visual);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < VisualTreeHelper.GetChildrenCount(iParent); j++)
                            {
                                Visual childVisual = (Visual)VisualTreeHelper.GetChild(iParent, j);

                                result.Add(childVisual);
                            }
                        }
                    }
                }));

                return result;
            }
        }
    }
}

