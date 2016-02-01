using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Messenger.Default.Register<Messages>(this, Messages.NotesChanged, EvaluateMessages);
        }

        private void EvaluateMessages(Messages obj)
        {
            if (obj == Messages.NotesChanged)
            {
                var vm = SimpleIoc.Default.GetInstance<MainViewModel>();
                
                //var adap2 = new TileBindingContentAdaptive()
                //{
                //    Children =
                //    {
                //        new TileText()
                //        {
                //            Style = TileTextStyle.Title,
                //            Text = vm.NewNotes.Count + " things to remember"
                //        },
                //        // For spacing
                //        new TileText()
                //    }
                //};

                //foreach (var noteModel in vm.NewNotes)
                //{
                //    adap2.Children.Add(new TileText()
                //    {
                //        Style = TileTextStyle.Body,
                //        Text = noteModel.Content
                //    });
                //}
                

                //var tile = new TileBinding()
                //{
                //    Branding = TileBranding.NameAndLogo,
                //    Content = adap2
                //};

                //var notif = new TileNotification(tile);

                //var tileTitle = string.Format("Local Notification {0}", i);
                //var tilesSubtitle = DateTime.UtcNow.AddHours(i);
                //var myTile = Generator.Generate(tileTitle, tilesSubtitle);

                //var notification = new TileNotification(myTile.ToXmlDoc()) { ExpirationTime = tilesSubtitle.AddMinutes(15), Tag = i.ToString() };

                //TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);

                //var updater = Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForApplication();
                //updater.Update(tile.);
            }
        }




        private static TileGroup CreateGroup(string from, string subject, string body)
        {
            return new TileGroup()
            {
                Children =
                {
                    new TileSubgroup()
                    {
                        Children =
                        {
                            new TileText()
                            {
                                Text = from,
                                Style = TileTextStyle.Subtitle
                            },

                            new TileText()
                            {
                                Text = subject,
                                Style = TileTextStyle.CaptionSubtle
                            },

                            new TileText()
                            {
                                Text = body,
                                Style = TileTextStyle.CaptionSubtle
                            }
                        }
                    }
                }
            };
        }
    }
}
