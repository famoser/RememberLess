using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.UI.Notifications;
using Famoser.RememberLess.View.Enums;
using Famoser.RememberLess.View.ViewModel;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using NotificationsExtensions.Tiles;

namespace Famoser.RememberLess.Presentation.WindowsUniversal.Services
{
    public class TilesService
    {
        public TilesService()
        {
            Messenger.Default.Register<Messages>(this, EvaluateMessages);
        }

        private void EvaluateMessages(Messages obj)
        {
            if (obj == Messages.NotesChanged)
            {
                var vm = SimpleIoc.Default.GetInstance<MainViewModel>();

                var adap2 = new TileBindingContentAdaptive()
                {
                    Children =
                    {
                        new TileText()
                        {
                            Style = TileTextStyle.Title,
                            Text = vm.NewNotes.Count + " pending"
                        },
                        // For spacing
                        new TileText()
                    }
                };

                foreach (var noteModel in vm.NewNotes)
                {
                    adap2.Children.Add(new TileText()
                    {
                        Style = TileTextStyle.BodySubtle,
                        Text = noteModel.Content
                    });
                }

                var tileLarge = new TileBinding()
                {
                    Branding = TileBranding.None,
                    Content = adap2
                };

                var tileSmall = new TileBinding()
                {
                    Branding = TileBranding.None,
                    Content = new TileBindingContentAdaptive()
                    {
                        Children =
                        {
                            new TileText()
                            {
                                Style = TileTextStyle.Header,
                                Text = vm.NewNotes.Count.ToString("00")
                            }
                        }
                    }
                };

                var tileVisual = new TileVisual()
                {
                    TileLarge = tileLarge,
                    TileMedium = tileLarge,
                    TileWide = tileLarge,
                    TileSmall= tileSmall,
                };
                
                var tileContent = new TileContent()
                {
                    Visual = tileVisual
                };
                
                var notif = new TileNotification(tileContent.GetXml());
                var updater = TileUpdateManager.CreateTileUpdaterForApplication();
                updater.Update(notif);

            }
        }
    }
}
