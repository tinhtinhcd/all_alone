using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace UI.MultiPage
{
    public partial class Manager : Control
    {
        /// Fields - protected or private ///
        protected Dictionary<String, Page> _pages = new Dictionary<String, Page>();
        private Page _curentPage;

        private Control _pageContainer;


        //////////////////////////////
        // Engine Callback Methods  //
        //////////////////////////////
        public override void _Ready()
        {
            _pageContainer = GetNode<Control>("Pages");
            Vector2 viewportSize = GetViewport().GetViewport().GetVisibleRect().Size;

            foreach (Page page in _pageContainer.GetChildren())
            {
                _pages.Add(page.Name, page);

                Vector2 pageViewportPosition = new Vector2(
                    page.Coordinates.X * viewportSize.X,
                    page.Coordinates.Y * viewportSize.Y
                );
                page.Position = pageViewportPosition;

                if (_curentPage == null)
                {
                    _curentPage = page;
                    _curentPage.Show();
                    _curentPage.IsActive = true;
                    _curentPage.Connect(
                        nameof(Page.ChangePageRequested), 
                        this, nameof(OnCurrentPageChangePageRequested)
                    );
                }
                else
                {
                    page.Hide();
                }
            }

            GetViewport().Connect("size_changed", new Callable(this, nameof(OnViewportSizeChanged)));
        }
        
        //////////////////////////////
        // Signal Connected Methods //
        //////////////////////////////
        private void OnViewportSizeChanged()
        {
            Vector2 viewportSize = GetViewport().GetViewport().GetVisibleRect().Size;
            
            // Move page container
            _pageContainer.Position = new Vector2(
                _curentPage.Coordinates.X * - viewportSize.X,
                _curentPage.Coordinates.Y * - viewportSize.Y
            );

            // Move all pages
            foreach (Page page in _pages.Values)
            {
                Vector2 pageViewportPosition = new Vector2(
                    page.Coordinates.X * viewportSize.X,
                    page.Coordinates.Y * viewportSize.Y
                );
                page.Position = pageViewportPosition;
            }
        }

        private void OnCurrentPageChangePageRequested(String targetPageName)
        {
            Debug.Assert(_pages.ContainsKey(targetPageName), "${targetPageName} is an invalid page name");
            ChangeCurrentPage(targetPageName);
        }


        //////////////////////////////
        //      Private Methods     //
        //////////////////////////////
        private async void ChangeCurrentPage(String tergetPageName)
        {
            Vector2 viewportSize = GetViewport().GetViewport().GetVisibleRect().Size;
            Page newPage = _pages[tergetPageName];
            Page oldPage = _curentPage;
            Vector2 coordinateChange = oldPage.Coordinates - newPage.Coordinates;
            Vector2 coordinateChangeViewport = new Vector2(
                coordinateChange.X * viewportSize.X,
                coordinateChange.Y * viewportSize.Y
            );

            // Deactivate old page
            oldPage.IsActive = false;
            oldPage.Disconnect(
                nameof(Page.ChangePageRequested), 
                 this, nameof(OnCurrentPageChangePageRequested)
            );

            newPage.Show();
            Tween pageMoveTween = CreateTween();
            pageMoveTween.TweenProperty(
                _pageContainer, "rect_position", coordinateChangeViewport, 1
            ).AsRelative().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quad);
            pageMoveTween.Play();
            await ToSignal(pageMoveTween, "finished");
            oldPage.Hide();

            // Activate new page
            _curentPage = newPage;
            _curentPage.IsActive = true;
            _curentPage.Connect(
                nameof(Page.ChangePageRequested), 
                 this, nameof(OnCurrentPageChangePageRequested)
            );
        }
    }
}

