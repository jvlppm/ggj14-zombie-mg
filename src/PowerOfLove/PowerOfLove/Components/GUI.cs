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
        #endregion

        #region Constructors
        public GUI()
        {
            _uiComponents = new List<Component>();
        }
        #endregion

        #region Game Loop
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(MainGame.DefaultBackgroundColor);
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
        public void Add(Component component)
        {
            _uiComponents.Add(component);
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
    }
}
