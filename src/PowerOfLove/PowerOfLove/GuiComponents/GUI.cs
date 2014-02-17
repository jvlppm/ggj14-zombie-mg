using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.GUI.Base;
using System.Collections.Generic;

namespace PowerOfLove.Components
{
    class GUI : ICollection<Component>
    {
        #region Attributes
        List<Component> _uiComponents;
        public readonly Vector2 Scale;
        #endregion

        #region Constructors
        public GUI(Vector2? scale = null)
        {
            Scale = scale ?? Vector2.One;
            _uiComponents = new List<Component>();
        }
        #endregion

        #region Game Loop
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

            foreach (var cmp in _uiComponents)
                cmp.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            foreach (var cmp in _uiComponents)
                cmp.Update(gameTime);
        }
        #endregion

        #region ICollection Methods
        public void Add(Component item)
        {
            FixScale(item);
            _uiComponents.Add(item);
        }

        public void Clear()
        {
            _uiComponents.Clear();
        }

        public bool Contains(Component item)
        {
            return _uiComponents.Contains(item);
        }

        public void CopyTo(Component[] array, int arrayIndex)
        {
            _uiComponents.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _uiComponents.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((ICollection<Component>)_uiComponents).IsReadOnly; }
        }

        public bool Remove(Component item)
        {
            item.Scale /= Scale;
            return _uiComponents.Remove(item);
        }

        public IEnumerator<Component> GetEnumerator()
        {
            return _uiComponents.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _uiComponents.GetEnumerator();
        }
        #endregion

        #region Private Methods
        void FixScale(Component item)
        {
            item.Scale *= Scale;
            if (item is Container)
                ((Container)item).Children.ForEach(FixScale);
        }

        #endregion
    }
}
