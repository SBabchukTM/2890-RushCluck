using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Runtime.Application.UserAccountSystem
{
    public class AvatarSelectionService
    {
        public async UniTask<Sprite> PickImage(int maxSize, CancellationToken cancellationToken = default)
        {
            var taskCompletionSource = new UniTaskCompletionSource<Sprite>();
            
            NativeGallery.Permission permission = NativeGallery.GetImageFromGallery(async (path) =>
            {
                if (string.IsNullOrEmpty(path))
                {
                    taskCompletionSource.TrySetResult(null);
                    return;
                }
    
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
                if (texture == null)
                {
                    taskCompletionSource.TrySetResult(null);
                    return;
                }
    
                var newAvatarSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                taskCompletionSource.TrySetResult(newAvatarSprite);
            });
            
            return await taskCompletionSource.Task.AttachExternalCancellation(cancellationToken);
        }
    }
}
