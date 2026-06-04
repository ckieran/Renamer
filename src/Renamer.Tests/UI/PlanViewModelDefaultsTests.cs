using Microsoft.Extensions.Logging.Abstractions;
using Renamer.Core.Contracts;
using Renamer.Core.Execution;
using Renamer.Core.Planning;
using Renamer.Core.Serialization;
using Renamer.UI.Plans;
using Renamer.UI.Resources.Strings;

namespace Renamer.Tests.UI;

public sealed class PlanViewModelDefaultsTests
{
    [Fact]
    public void PlanFileName_DefaultsToRenameJson()
    {
        var vm = MakeViewModel();

        Assert.Equal(AppStrings.DefaultPlanFileName, vm.PlanFileName);
    }

    [Fact]
    public void GenerationOutputDirectoryPath_WhenRootSet_AutoFillsToRoot()
    {
        var vm = MakeViewModel();

        vm.GenerationRootPath = "/photos";

        Assert.Equal("/photos", vm.GenerationOutputDirectoryPath);
    }

    [Fact]
    public void GenerationOutputDirectoryPath_WhenRootChanges_FollowsRootIfNotOverridden()
    {
        var vm = MakeViewModel();
        vm.GenerationRootPath = "/photos";

        vm.GenerationRootPath = "/pictures";

        Assert.Equal("/pictures", vm.GenerationOutputDirectoryPath);
    }

    [Fact]
    public async Task GenerationOutputDirectoryPath_WhenUserBrowses_IsPreservedOnRootChange()
    {
        var vm = MakeViewModel(outputPickerResult: "/custom/output");
        vm.GenerationRootPath = "/photos";

        await vm.BrowseGenerationOutputDirectoryAsync();
        vm.GenerationRootPath = "/pictures";

        Assert.Equal("/custom/output", vm.GenerationOutputDirectoryPath);
    }

    [Fact]
    public void HasAdvancedOverrides_DefaultsToFalse()
    {
        var vm = MakeViewModel();

        Assert.False(vm.HasAdvancedOverrides);
    }

    [Fact]
    public void GenerationStatusMessage_DefaultsToPhotoFolderOnlyPrompt()
    {
        var vm = MakeViewModel();

        Assert.Equal("Choose a photo folder to build a plan.", vm.GenerationStatusMessage);
    }

    [Fact]
    public void HasAdvancedOverrides_WhenOutputAutoFilled_RemainsFlase()
    {
        var vm = MakeViewModel();

        vm.GenerationRootPath = "/photos";

        Assert.False(vm.HasAdvancedOverrides);
    }

    [Fact]
    public async Task HasAdvancedOverrides_WhenUserBrowsesOutput_BecomesTrue()
    {
        var vm = MakeViewModel(outputPickerResult: "/custom/output");

        await vm.BrowseGenerationOutputDirectoryAsync();

        Assert.True(vm.HasAdvancedOverrides);
    }

    [Fact]
    public void HasAdvancedOverrides_WhenPlanFileNameChanged_BecomesTrue()
    {
        var vm = MakeViewModel();

        vm.PlanFileName = "my-plan.json";

        Assert.True(vm.HasAdvancedOverrides);
    }

    [Fact]
    public void HasAdvancedOverrides_RaisesPropertyChanged_WhenOutputOverridden()
    {
        var vm = MakeViewModel();
        var raised = new List<string?>();
        vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        vm.GenerationOutputDirectoryPath = "/custom/output";

        Assert.Contains(nameof(vm.HasAdvancedOverrides), raised);
    }

    [Fact]
    public void HasAdvancedOverrides_RaisesPropertyChanged_WhenFileNameOverridden()
    {
        var vm = MakeViewModel();
        var raised = new List<string?>();
        vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        vm.PlanFileName = "my-plan.json";

        Assert.Contains(nameof(vm.HasAdvancedOverrides), raised);
    }

    private static PlanViewModel MakeViewModel(string? outputPickerResult = null) =>
        new(
            new FakePlanBuilder(),
            new FakePlanSerializer(),
            new FakePlanFilePicker(),
            new FakeFolderPathPicker(outputPickerResult),
            new FakeRootPathOpener(),
            new FakeApplyEngine(),
            NullLogger<PlanViewModel>.Instance);

    private sealed class FakePlanBuilder : IPlanBuilder
    {
        public RenamePlan Build(string rootPath) => throw new NotImplementedException();
    }

    private sealed class FakePlanSerializer : IPlanSerializer
    {
        public RenamePlan Read(string inputPath) => throw new NotImplementedException();
        public void Write(string outputPath, RenamePlan plan) => throw new NotImplementedException();
    }

    private sealed class FakePlanFilePicker : IPlanFilePicker
    {
        public Task<string?> PickPlanPathAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<string?>(null);
    }

    private sealed class FakeFolderPathPicker(string? result) : IFolderPathPicker
    {
        public Task<string?> PickFolderPathAsync(string title, CancellationToken cancellationToken = default) =>
            Task.FromResult(result);
    }

    private sealed class FakeRootPathOpener : IRootPathOpener
    {
        public Task OpenAsync(string directoryPath, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    private sealed class FakeApplyEngine : IApplyEngine
    {
        public RenameReport Execute(RenamePlan plan) => throw new NotImplementedException();
    }
}
