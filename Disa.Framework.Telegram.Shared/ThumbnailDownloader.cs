﻿using System;
using SharpTelegram.Schema;
using Disa.Framework.Bubbles;

namespace Disa.Framework.Telegram
{
    public partial class Telegram : IThumbnailDownloader
    {
        public Tuple<bool,byte[]> FetchQuotedThumbnailBytes(VisualBubble bubble)
        {
            return GetThumbnailBytes(bubble.QuotedIdService);
        }

        public Tuple<bool,byte[]> FetchThumbnailBytes(VisualBubble bubble)
        {
            return GetThumbnailBytes(bubble.IdService);
        }

        private Tuple<bool,byte[]> GetThumbnailBytes(string id)
        {
            if (!Platform.HasInternetConnection())
            {
                return Tuple.Create<bool, byte[]>(false, null);
            }

            if (id == null)
            {
                return Tuple.Create<bool,byte[]>(true,null);
            }
            using (var client = new FullClientDisposable(this))
            {

                var iMessage = GetMessage(uint.Parse(id), client.Client);

                var message = iMessage as Message;
                if (message != null)
                {
                    var messageMediaPhoto = message.Media as MessageMediaPhoto;
                    if (messageMediaPhoto != null)
                    {
                        var bytes =  GetCachedPhotoBytes(messageMediaPhoto.Photo);
                        if (bytes == null)
                        {
                            //try to get bytes from the server
                            var fileLocation = GetCachedPhotoFileLocation(messageMediaPhoto.Photo);
                            DebugPrint(">>>> Filelocation " + ObjectDumper.Dump(fileLocation));
                            var bytesSecond = FetchFileBytes(fileLocation);
                            if (bytesSecond != null)
                            {
                                return Tuple.Create(true, bytesSecond);
                            }
                        }
                        return Tuple.Create(true, bytes);
                    }

                }
                return Tuple.Create<bool, byte[]>(true, null);
            }
        }
    }
}

