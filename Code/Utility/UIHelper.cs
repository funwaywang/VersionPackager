using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VPackager
{
    public static class UIHelper
    {
        public static T TryFindAncestor<T>(this DependencyObject element, bool withSelf = true)
            where T : DependencyObject
        {
            if (element == null)
                return null;

            if (withSelf && element is T)
                return (T)element;

            var parentObject = GetParentObject(element);
            if (parentObject != element)
            {
                while (parentObject != null)
                {
                    if (parentObject is T)
                        return (T)parentObject;
                    parentObject = GetParentObject(parentObject);
                }
            }

            return null;
        }

        public static UIElement TryFindAncestor(this UIElement element, Predicate<UIElement> predicate, bool withSelf)
        {
            if (element == null)
                return null;

            if (withSelf && predicate(element))
                return element;

            var parentObject = GetParentObject(element);
            if (parentObject != element)
            {
                while (parentObject != null)
                {
                    if (parentObject is UIElement && predicate((UIElement)parentObject))
                        return (UIElement)parentObject;
                    parentObject = GetParentObject(parentObject);
                }
            }

            return null;
        }

        public static FrameworkElement TryFindAncestorByData<T>(this UIElement element, bool withSelf = true)
        {
            if (element == null)
                return null;

            if (withSelf && element is FrameworkElement && ((FrameworkElement)element).DataContext is T)
                return (FrameworkElement)element;

            var parentObject = GetParentObject(element) as UIElement;
            if (parentObject != element)
            {
                while (parentObject != null)
                {
                    if (parentObject is FrameworkElement && ((FrameworkElement)parentObject).DataContext is T)
                        return (FrameworkElement)parentObject;
                    parentObject = GetParentObject(parentObject) as FrameworkElement;
                }
            }

            return null;
        }

        public static DependencyObject GetParentObject(this DependencyObject element)
        {
            if (element == null)
                return null;

            if (element is ContentElement)
            {
                var parent = ContentOperations.GetParent((ContentElement)element);
                if (parent != null)
                    return parent;

                var fce = element as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            if (element is FrameworkElement)
            {
                var parent = ((FrameworkElement)element).Parent;
                if (parent != null)
                    return parent;
            }

            return VisualTreeHelper.GetParent(element);
        }

        public static Visual GetDescendantByName(this Visual element, string name)
        {
            if (element == null) return null;

            if (element is FrameworkElement && StringComparer.OrdinalIgnoreCase.Equals((element as FrameworkElement).Name, name))
                return element;

            Visual result = null;
            if (element is FrameworkElement)
                (element as FrameworkElement).ApplyTemplate();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                result = GetDescendantByName(visual, name);
                if (result != null)
                    break;
            }

            return result;
        }

        public static Visual GetDescendant<T>(this Visual element)
            where T : Visual
        {
            if (element is T)
                return (T)element;

            Visual foundElement = null;
            if (element is FrameworkElement)
                (element as FrameworkElement).ApplyTemplate();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = GetDescendant<T>(visual);
                if (foundElement != null)
                {
                    return foundElement;
                }
            }

            return null;
        }

        public static DependencyObject[] HitTestRange(this FrameworkElement container, Rect range)
        {
            var selectedElements = new List<DependencyObject>();

            var rect = new RectangleGeometry(range);
            var hitTestParams = new GeometryHitTestParameters(rect);
            var resultCallback = new HitTestResultCallback(result => HitTestResultBehavior.Continue);
            var filterCallback = new HitTestFilterCallback(element =>
            {
                if (VisualTreeHelper.GetParent(element) == container)
                {
                    selectedElements.Add(element);
                }
                return HitTestFilterBehavior.Continue;
            });

            VisualTreeHelper.HitTest(container, filterCallback, resultCallback, hitTestParams);

            return selectedElements.ToArray();
        }

        public static T[] HitTestRange<T>(this FrameworkElement container, Rect range)
            where T : DependencyObject
        {
            var selectedElements = new List<T>();

            var rect = new RectangleGeometry(range);
            var hitTestParams = new GeometryHitTestParameters(rect);
            var resultCallback = new HitTestResultCallback(result => HitTestResultBehavior.Continue);
            var filterCallback = new HitTestFilterCallback(element =>
            {
                if (element is T)
                {
                    selectedElements.Add((T)element);
                }
                return HitTestFilterBehavior.Continue;
            });

            VisualTreeHelper.HitTest(container, filterCallback, resultCallback, hitTestParams);

            return selectedElements.ToArray();
        }

        public static DependencyObject GetItemContainerAtPoint(this ItemsControl control, Point point)
        {
            var result = VisualTreeHelper.HitTest(control, point);
            if (result.VisualHit != null)
                return control.ContainerFromElement(result.VisualHit);
            else
                return null;
        }

        public static TItemContainer GetItemContainerAtPoint<TItemContainer>(this ItemsControl control, Point point)
            where TItemContainer : DependencyObject
        {
            var result = VisualTreeHelper.HitTest(control, point);
            if (result.VisualHit != null)
                return control.ContainerFromElement(result.VisualHit) as TItemContainer;
            else
                return null;
        }
    }
}
