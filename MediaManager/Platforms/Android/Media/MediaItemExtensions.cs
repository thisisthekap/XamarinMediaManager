﻿using System;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Media;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Source.Dash;
using Com.Google.Android.Exoplayer2.Source.Hls;
using Com.Google.Android.Exoplayer2.Source.Smoothstreaming;
using MediaManager.Media;

namespace MediaManager.Platforms.Android.Media
{
    public static class MediaItemExtensions
    {
        public static IMediaSource ToMediaSource(this IMediaItem mediaItem)
        {
            if (MediaPlayer.DataSourceFactory == null)
                throw new ArgumentNullException(nameof(MediaPlayer.DataSourceFactory));

            IMediaSource mediaSource;
            switch (mediaItem.MediaType)
            {
                default:
                case MediaType.Default:
                    mediaSource = new ExtractorMediaSource.Factory(MediaPlayer.DataSourceFactory)
                        .SetTag(mediaItem.ToMediaDescription())
                        .CreateMediaSource(global::Android.Net.Uri.Parse(mediaItem.MediaUri));
                    break;
                case MediaType.Dash:
                    if (MediaPlayer.DashChunkSourceFactory == null)
                        throw new ArgumentNullException(nameof(MediaPlayer.DashChunkSourceFactory));

                    mediaSource = new DashMediaSource.Factory(MediaPlayer.DashChunkSourceFactory, MediaPlayer.DataSourceFactory)
                        .SetTag(mediaItem.ToMediaDescription())
                        .CreateMediaSource(global::Android.Net.Uri.Parse(mediaItem.MediaUri));
                    break;
                case MediaType.Hls:
                    mediaSource = new HlsMediaSource.Factory(MediaPlayer.DataSourceFactory)
                        .SetAllowChunklessPreparation(true)
                        .SetTag(mediaItem.ToMediaDescription())
                        .CreateMediaSource(global::Android.Net.Uri.Parse(mediaItem.MediaUri));
                    break;
                case MediaType.SmoothStreaming:
                    if (MediaPlayer.SsChunkSourceFactory == null)
                        throw new ArgumentNullException(nameof(MediaPlayer.SsChunkSourceFactory));

                    mediaSource = new SsMediaSource.Factory(MediaPlayer.SsChunkSourceFactory, MediaPlayer.DataSourceFactory)
                        .SetTag(mediaItem.ToMediaDescription())
                        .CreateMediaSource(global::Android.Net.Uri.Parse(mediaItem.MediaUri));
                    break;
            }

            return mediaSource;
        }

        public static MediaDescriptionCompat ToMediaDescription(this IMediaItem item)
        {
            var description = new MediaDescriptionCompat.Builder()
                .SetMediaId(item?.MediaId)
                .SetMediaUri(global::Android.Net.Uri.Parse(item?.MediaUri))
                .SetTitle(item?.GetTitle())
                .SetSubtitle(item?.GetContentTitle())
                .SetDescription(item?.DisplayDescription)
                .SetExtras(item?.Extras as Bundle)
                .SetIconBitmap(item?.GetCover())
                .SetIconUri(!string.IsNullOrEmpty(item?.DisplayIconUri) ? global::Android.Net.Uri.Parse(item?.DisplayIconUri) : null)
                .Build();

            return description;
        }

        public static MediaBrowserCompat.MediaItem ToMediaBrowserMediaItem(this IMediaItem item)
        {
            var media = new MediaBrowserCompat.MediaItem(ToMediaDescription(item), MediaBrowserCompat.MediaItem.FlagPlayable);
            return media;
        }

        public static IMediaItem ToMediaItem(this MediaDescriptionCompat mediaDescription)
        {
            var item = new MediaItem(mediaDescription.MediaUri.ToString());
            //item.Advertisement = mediaDescription.
            //item.Album = mediaDescription.
            //item.AlbumArt = mediaDescription.
            //item.AlbumArtist = mediaDescription.
            //item.AlbumArtUri = mediaDescription.
            //item.Art = mediaDescription.
            //item.Artist = mediaDescription.
            //item.ArtUri = mediaDescription.
            //item.Author = mediaDescription.
            //item.BtFolderType = mediaDescription.
            //item.Compilation = mediaDescription.
            //item.Composer = mediaDescription.
            //item.Date = mediaDescription.
            //item.DiscNumber = mediaDescription.
            //item.DisplayDescription = mediaDescription.
            item.DisplayIcon = mediaDescription.IconBitmap;
            item.DisplayIconUri = mediaDescription.IconUri.ToString();
            item.DisplaySubtitle = mediaDescription.Subtitle;
            item.DisplayTitle = mediaDescription.Title;
            //item.DownloadStatus = mediaDescription.
            //item.Duration = mediaDescription.
            item.Extras = mediaDescription.Extras;
            //item.Genre = mediaDescription.
            item.MediaId = mediaDescription.MediaId;
            item.MediaUri = mediaDescription.MediaUri.ToString();
            //item.NumTracks = mediaDescription.
            //item.Rating = mediaDescription.
            item.Title = mediaDescription.Title;
            //item.TrackNumber = mediaDescription.
            //item.UserRating = mediaDescription.
            //item.Writer = mediaDescription.
            //item.Year = mediaDescription.
            item.IsMetadataExtracted = true;
            return item;
        }

        public static IMediaItem ToMediaItem(this MediaMetadataCompat mediaMetadata)
        {
            var item = new MediaItem(mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyMediaUri));
            item.Advertisement = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyAdvertisement);
            item.Album = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyAlbum);
            item.AlbumArt = mediaMetadata.GetBitmap(MediaMetadataCompat.MetadataKeyAlbumArt);
            item.AlbumArtist = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyAlbumArtist);
            item.AlbumArtUri = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyAlbumArtUri);
            item.Art = mediaMetadata.GetBitmap(MediaMetadataCompat.MetadataKeyArt);
            item.Artist = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyArtist);
            item.ArtUri = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyArtUri);
            item.Author = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyAuthor);
            //item.BtFolderType = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyBtFolderType);
            item.Compilation = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyCompilation);
            item.Composer = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyComposer);
            item.Date = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyDate);
            item.DiscNumber = Convert.ToInt32(mediaMetadata.GetLong(MediaMetadataCompat.MetadataKeyDiscNumber));
            item.DisplayDescription = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyDisplayDescription);
            item.DisplayIcon = mediaMetadata.GetBitmap(MediaMetadataCompat.MetadataKeyDisplayIcon);
            item.DisplayIconUri = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyDisplayIconUri);
            item.DisplaySubtitle = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyDisplaySubtitle);
            item.DisplayTitle = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyDisplayTitle);
            item.DownloadStatus = mediaMetadata.GetLong(MediaMetadataCompat.MetadataKeyDownloadStatus) == 0 ? DownloadStatus.NotDownloaded : DownloadStatus.Downloaded;
            item.Duration = TimeSpan.FromMilliseconds(Convert.ToInt32(mediaMetadata.GetLong(MediaMetadataCompat.MetadataKeyDuration)));
            //item.Extras = mediaMetadata.GetString(MediaMetadataCompat.extr);
            item.Genre = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyGenre);
            item.MediaId = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyMediaId);
            item.MediaUri = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyMediaUri);
            item.NumTracks = Convert.ToInt32(mediaMetadata.GetLong(MediaMetadataCompat.MetadataKeyNumTracks));
            item.Rating = mediaMetadata.GetRating(MediaMetadataCompat.MetadataKeyRating);
            item.Title = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyTitle);
            item.TrackNumber = Convert.ToInt32(mediaMetadata.GetLong(MediaMetadataCompat.MetadataKeyTrackNumber));
            item.UserRating = mediaMetadata.GetRating(MediaMetadataCompat.MetadataKeyUserRating);
            item.Writer = mediaMetadata.GetString(MediaMetadataCompat.MetadataKeyWriter);
            item.Year = Convert.ToInt32(mediaMetadata.GetLong(MediaMetadataCompat.MetadataKeyYear));
            item.IsMetadataExtracted = true;
            return item;
        }

        public static Bitmap GetCover(this IMediaItem mediaItem)
        {
            if (mediaItem.AlbumArt is Bitmap bitmap)
                return bitmap;
            else if (mediaItem.Art is Bitmap artBitmap)
                return artBitmap;
            else if (!string.IsNullOrEmpty(mediaItem.ArtUri))
            {
                return GetImageBitmapFromUrl(mediaItem.ArtUri);
            }
            else if (!string.IsNullOrEmpty(mediaItem.AlbumArtUri))
            {
                return GetImageBitmapFromUrl(mediaItem.AlbumArtUri);
            }
            return null;
        }

        private static Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new System.Net.WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }
    }
}
