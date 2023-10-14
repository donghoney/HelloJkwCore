﻿
namespace GameLibra.Page;

public partial class LibraBoard : JkwPageBase
{
    [Parameter] public string GameId { get; set; }
    [Inject] public ILibraService LibraService { get; set; }
    [Inject] public ISnackbar Snackbar { get; set; }
    [Inject] public IDialogService DialogService { get; set; }

    GameEngine _gameEngine;
    LibraGameState _state;
    List<DropCubeItem> _cubes;
    Player _currentPlayer;
    LibraBoardSetting Setting = new();

    protected override Task OnPageInitializedAsync()
    {
        if (IsAuthenticated)
        {
            _gameEngine = LibraService.GetGame(GameId);
            if (_gameEngine == null)
            {
                Navi.NavigateTo("/game/libra/home");
                return Task.CompletedTask;
            }
            _gameEngine.StateChanged += GameEngine_StateChanged;
            _state = _gameEngine?.State;
            _cubes = GetCubes(_state);
            _currentPlayer = _state.Players.FirstOrDefault(x => x.Id == _state.CurrentPlayerId);
        }
        else
        {
            Navi.NavigateTo("/login");
        }
        return base.OnPageInitializedAsync();
    }

    private void GameEngine_StateChanged(object sender, LibraGameState e)
    {
        InvokeAsync(() =>
        {
            _state = e;
            _cubes = GetCubes(_state);
            _currentPlayer = _state.Players.FirstOrDefault(x => x.Id == _state.CurrentPlayerId);
            StateHasChanged();
        });
    }

    protected override void OnPageDispose()
    {
        if (_gameEngine != null)
        {
            _gameEngine.StateChanged -= GameEngine_StateChanged;
        }
        base.OnPageDispose();
    }

    private List<DropCubeItem> GetCubes(LibraGameState state)
    {
        return state?.Players
            .SelectMany(p => GetCubes(p))
            .ToList();

        List<DropCubeItem> GetCubes(Player player)
        {
            return player.Cubes
                .Select((cube, i) => new DropCubeItem
                {
                    Cube = cube,
                    Origin = $"player-{player.Id}",
                    Identifier = $"player-{player.Id}",
                })
                .ToList();
        }
    }
    private bool CanDrop(DropCubeItem item, string targetIdentifier)
    {
        if (item.Identifier.Contains("player"))
        {
            return targetIdentifier.Contains("scale");
        }
        else if (item.Identifier.Contains("scale"))
        {
            if (targetIdentifier.Contains("scale"))
            {
                return true;
            }
            else
            {
                return item.Origin == targetIdentifier;
            }
        }
        else
        {
            return false;
        }
    }
    private void ItemDropped(MudItemDropInfo<DropCubeItem> dropItem)
    {
        dropItem.Item.Identifier = dropItem.DropzoneIdentifier;
    }
    private async Task SettingChanged(LibraBoardSetting setting)
    {
        await InvokeAsync(() =>
        {
            Setting = setting;
            StateHasChanged();
        });
    }
}

