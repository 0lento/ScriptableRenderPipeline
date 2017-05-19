using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace UnityEditor.MaterialGraph.Drawing
{
    public class NodePreviewDrawer : Image
    {
        public NodePreviewPresenter data;

        public override void DoRepaint(IStylePainter painter)
        {
            image = data.Render(new Vector2(200, 200));

            painter.DrawTexture(position, image, new Color(1, 1, 1, 1), scaleMode);
            //base.DoRepaint(args);
        }
    }
}
