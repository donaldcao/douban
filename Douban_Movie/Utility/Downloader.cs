﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;
using System.IO;

namespace PanoramaApp2.Utility
{
    class Downloader
    {
        private String url;
        private HttpClient httpClient;
        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;

        /// <summary>
        /// Downloader constructor with URL
        /// </summary>
        /// <param name="url">URL to download content</param>
        public Downloader(String url)
        {
            this.url = url;
            httpClient = new HttpClient();
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
        }

        /// <summary>
        /// Download string
        /// </summary>
        /// <returns>String downloaded</returns>
        public async Task<String> downloadString()
        {
            var response = await httpClient.GetAsync(url, cancellationToken);
            String downloadContent = await response.Content.ReadAsStringAsync();
            return downloadContent;
        }

        /// <summary>
        /// Download stream
        /// </summary>
        /// <returns>Stream downloaded</returns>
        public async Task<Stream> downloadStream()
        {
            var response = await httpClient.GetAsync(url, cancellationToken);
            Stream downloadContent = await response.Content.ReadAsStreamAsync();
            return downloadContent;
        }

        /// <summary>
        /// Download byte array
        /// </summary>
        /// <returns>Byte array downloaded</returns>
        public async Task<byte[]> downloadByteArray()
        {
            return await httpClient.GetByteArrayAsync(url);
        }

        /// <summary>
        /// Cancel download
        /// </summary>
        public void cancelDownload()
        {
            cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// If cancellation is requested
        /// </summary>
        /// <returns>If cancellation is requested</returns>
        public bool isCanceled()
        {
            return cancellationTokenSource.IsCancellationRequested;
        }
    }
}
