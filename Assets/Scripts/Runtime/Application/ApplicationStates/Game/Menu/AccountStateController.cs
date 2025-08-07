using System.Threading;
using Core.StateMachine;
using Runtime.UI;
using Cysharp.Threading.Tasks;
using ILogger = Core.ILogger;
using Runtime.Services.UserData;
using System;
using UnityEngine;
using Runtime.Application.UserAccountSystem;

namespace Runtime.Game
{
    public class AccountStateController : StateController
    {
        private readonly IUiService _uiService;
        private readonly UserDataService _userDataService;
        private readonly UserAccountService _userAccountService;
        private readonly AvatarSelectionService _avatarSelectionService;
        private readonly StartSettingsController _startSettingsController;

        private AccountScreen _screen;

        private UserAccountData _newData;
        private UserAccountData _modifiedData;

        public AccountStateController(
            ILogger logger,
            IUiService uiService,
            UserDataService userDataService,
            UserAccountService userAccountService,
             AvatarSelectionService avatarSelectionService,
            StartSettingsController startSettingsController) : base(logger)
        {
            _uiService = uiService;
            _userDataService = userDataService;
            _userAccountService = userAccountService;
            _avatarSelectionService = avatarSelectionService;
            _startSettingsController = startSettingsController;
        }

        public override UniTask Enter(CancellationToken cancellationToken)
        {
            CopyData();
            CreateScreen();
            SubscribeToEvents();

            return UniTask.CompletedTask;
        }

        public override async UniTask Exit()
        {
            _screen.OnNameChanged -= SetNewName;
            _screen.OnAgeChanged -= SetNewAge;
            _screen.OnGenderChanged -= SetNewGender;

            await _uiService.HideScreen(ConstScreens.AccountScreen);
        }

        private void CopyData()
        {
            _modifiedData = _userAccountService.GetAccountDataCopy();

            _newData = new UserAccountData()
            {
                Username = _modifiedData.Username,
                Age = _modifiedData.Age,
                Gender = _modifiedData.Gender,
            };
        }

        private void CreateScreen()
        {
            _screen = _uiService.GetScreen<AccountScreen>(ConstScreens.AccountScreen);
            _screen.Initialize();
            _screen.ShowAsync().Forget();
            _screen.SetData(_modifiedData);
            _screen.SetAvatar(_userAccountService.GetUsedAvatarSprite(false));
        }

        private void SubscribeToEvents()
        {
            _screen.OnBackPressed += async () => await GoTo<MenuStateController>();
            _screen.OnSavePressed += async () =>
            {
                SaveProfile();
                await GoTo<MenuStateController>();
            };

            _screen.OnChangeAvatarPressed += SelectNewAvatar;
            _screen.OnNameChanged += SetNewName;
            _screen.OnAgeChanged += SetNewAge;
            _screen.OnGenderChanged += SetNewGender;
        }

        private void SaveProfile()
        {
            ValidateName();
            ValidateAge();
            ValidateGender();
            _userAccountService.SaveAccountData(_modifiedData);
        }

        private async void SelectNewAvatar()
        {
            Sprite newAvatar = await _avatarSelectionService.PickImage(512, CancellationToken.None);

            if (newAvatar != null)
            {
                Texture2D originalTexture = newAvatar.texture;
                Texture2D readableTexture = CreateReadableTexture(originalTexture);
                Texture2D cropped = CropToSquare(readableTexture);

                Sprite squareSprite = Sprite.Create(
                    cropped,
                    new Rect(0, 0, cropped.width, cropped.height),
                    new Vector2(0.5f, 0.5f)
                );

                _modifiedData.AvatarBase64 = _userAccountService.ConvertToBase64(squareSprite);

                _screen.SetAvatar(squareSprite);
            }
        }

        private Texture2D CropToSquare(Texture2D source)
        {
            int size = Mathf.Min(source.width, source.height);
            int startX = (source.width - size) / 2;
            int startY = (source.height - size) / 2;

            Color[] pixels = source.GetPixels(startX, startY, size, size);
            Texture2D croppedTexture = new Texture2D(size, size);
            croppedTexture.SetPixels(pixels);
            croppedTexture.Apply();
            return croppedTexture;
        }

        private Texture2D CreateReadableTexture(Texture2D original)
        {
            RenderTexture tmp = RenderTexture.GetTemporary(
                original.width,
                original.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);

            Graphics.Blit(original, tmp);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tmp;

            Texture2D readableTexture = new Texture2D(original.width, original.height);
            readableTexture.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            readableTexture.Apply();

            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(tmp);

            return readableTexture;
        }

        private void ValidateGender()
        {
            var value = _newData.Gender;
            if (value.Length < 2)
            {
                _screen.SetData(_modifiedData);
                return;
            }

            _modifiedData.Gender = value;
        }

        private void ValidateAge()
        {
            try
            {
                var value = _newData.Age;
                int age = Convert.ToInt32(value);
                if (age < 18 || age > 99)
                {
                    _screen.SetData(_modifiedData);
                    return;
                }

                _modifiedData.Age = age;
            }
            catch
            {
            }
        }

        private void ValidateName()
        {
            var value = _newData.Username;
            if (value.Length < 2)
            {
                _screen.SetData(_modifiedData);
                return;
            }

            _modifiedData.Username = value;
        }

        private void SetNewName(string value)
        {
            _newData.Username = value;
        }

        private void SetNewAge(string value)
        {
            try
            {
                int age = Convert.ToInt32(value);
                _newData.Age = age;
            }
            catch
            {
            }
        }

        private void SetNewGender(string value)
        {
            _newData.Gender = value;
        }
    }
}