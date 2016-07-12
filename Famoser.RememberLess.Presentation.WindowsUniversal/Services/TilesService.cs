using System.Linq;
using Windows.UI.Notifications;
using Famoser.RememberLess.View.Enums;
using Famoser.RememberLess.View.ViewModel;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using NotificationsExtensions;
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
                if (vm != null && vm.NoteCollections != null)
                {
                    var newNotes = vm.NoteCollections.SelectMany(noteCollectionModel => noteCollectionModel.NewNotes).ToList();
                    var adap2 = new TileBindingContentAdaptive()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                HintStyle = AdaptiveTextStyle.Title,
                                Text = newNotes.Count + " pending"
                            },
                            // For spacing
                            new AdaptiveText()
                        }
                    };

                    foreach (var noteModel in newNotes)
                    {
                        adap2.Children.Add(new AdaptiveText()
                        {
                            Text = noteModel.Content,
                            HintStyle = AdaptiveTextStyle.BodySubtle
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
                                new AdaptiveText()
                                {
                                    HintStyle = AdaptiveTextStyle.Header,
                                    Text = newNotes.Count.ToString("00"),
                                    HintAlign = AdaptiveTextAlign.Center
                                }
                            }
                        }
                    };

                    var tileVisual = new TileVisual()
                    {
                        TileLarge = tileLarge,
                        TileMedium = tileLarge,
                        TileWide = tileLarge,
                        TileSmall = tileSmall,
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
}
