﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SyncClipboard.Core.Interfaces;
using SyncClipboard.Utility;
using SyncClipboard.Utility.Notification;
using static SyncClipboard.Service.ProfileType;
using Button = SyncClipboard.Utility.Notification.Button;
#nullable enable

namespace SyncClipboard.Service
{
    public class TextProfile : Profile
    {
        public TextProfile(String text)
        {
            Text = text;
        }

        public override ClipboardType GetProfileType()
        {
            return ClipboardType.Text;
        }

        public override string ToolTip()
        {
            return Text;
        }

        protected override Task<bool> Same(Profile rhs, CancellationToken cancellationToken)
        {
            try
            {
                var textprofile = (TextProfile)rhs;
                return Task.FromResult(Text == textprofile.Text);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public override async Task UploadProfileAsync(IWebDav webdav, CancellationToken cancelToken)
        {
            await webdav.PutText(SyncService.REMOTE_RECORD_FILE, this.ToJsonString(), cancelToken);
        }

        protected override DataObject CreateDataObject()
        {
            var dataObject = new DataObject();
            dataObject.SetData(DataFormats.Text, this.Text);
            return dataObject;
        }

        protected override void AfterSetLocal()
        {
            if (Text[..4] == "http" || Text[..4] == "www.")
            {
                Callbacker callbacker = new(Guid.NewGuid().ToString(), (_) => Sys.OpenWithDefaultApp(Text));
                Toast.SendText("文本同步成功", Text, DefaultButton(), new Button("在浏览器中打开", callbacker));
            }
            else
            {
                Toast.SendText("文本同步成功", Text, DefaultButton());
            }
        }
    }
}
