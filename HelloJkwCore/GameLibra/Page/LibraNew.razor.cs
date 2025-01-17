using System.Reflection;

namespace GameLibra.Page;

public partial class LibraNew : JkwPageBase
{
    [Inject] public ILibraService LibraService { get; set; }

    private string _name;
    private int _cubeCount = 5;
    private int _playerCount = 7;
    private int _scaleCount = 2;
    private int _cubeMinValue = 1;
    private int _cubeMaxValue = 20;
    private int _cubePerPlayer = 2;
    private int _minimumApplyCubeCount = 2;
    private int _timeoverSeconds = 300;
    private bool _visibleOtherCube = false;
    private bool _maybeUseAssist = true;
    private LibraGameHint _hint = LibraGameHint.None;

    private bool _moreOption = false;

    private List<(LibraGameHint HintValue, string HintDescription)> _hintList = new();

    protected override Task OnPageInitializedAsync()
    {
        _hintList = Enum.GetValues<LibraGameHint>()
            .Select(hintValue =>
            {
                string hintName = Enum.GetName<LibraGameHint>(hintValue);
                FieldInfo fieldInfo = typeof(LibraGameHint).GetField(hintName);
                HintAttribute hintAttribute = fieldInfo.GetCustomAttribute<HintAttribute>();
                return (hintValue, hintAttribute?.Description ?? string.Empty);
            })
            .ToList();
        return base.OnPageInitializedAsync();
    }

    private async Task CreateGameWithDevilsPlan()
    {
        if (!string.IsNullOrWhiteSpace(_name))
        {
            var engine = await LibraService.CreateGameWithDevilsPlan(User, _name);
            NavigationManager.NavigateTo($"game/libra/room/{engine.State.Id}");
        }
    }

    private async Task CreateGame()
    {
        if (!string.IsNullOrWhiteSpace(_name))
        {
            var rule = MakeRule();
            var engine = await LibraService.CreateGame(User, _name, rule);
            NavigationManager.NavigateTo($"game/libra/room/{engine.State.Id}");
        }
    }

    private async Task CreateGame2()
    {
        if (!string.IsNullOrWhiteSpace(_name))
        {
            var rule = new LibraGameRule
            {
                CubeCount = 5,
                PlayerCount = 7,
                ScaleCount = 2,
                CubeMinValue = 1,
                CubeMaxValue = 30,
                CubePerPlayer = 2,
                MinimumApplyCubeCount = 1,
                VisibleOtherCube = true,
                TimeOverSeconds = 300,
                Hint = LibraGameHint.None,
                MaybeUseAssist = true,
            };
            var engine = await LibraService.CreateGame(User, _name, rule);
            NavigationManager.NavigateTo($"game/libra/room/{engine.State.Id}");
        }
    }

    private LibraGameRule MakeRule()
    {
        return new LibraGameRule
        {
            CubeCount = _cubeCount,
            PlayerCount = _playerCount,
            ScaleCount = _scaleCount,
            CubeMinValue = _cubeMinValue,
            CubeMaxValue = _cubeMaxValue,
            CubePerPlayer = _cubePerPlayer,
            MinimumApplyCubeCount = _minimumApplyCubeCount,
            VisibleOtherCube = _visibleOtherCube,
            TimeOverSeconds = _timeoverSeconds,
            Hint = _hint,
            MaybeUseAssist = _maybeUseAssist,
        };
    }
}