using RedMangaCleanerCGUI.WPFDesign.Code.Xaml.MVVM.StartMenuView.View;
using RedMangaCleanerPROR.Code.Structures;
using System;
using System.Windows;
using System.Windows.Controls;

public class ImagesProcessingProgressInfo
{
    public bool IsFinished { get; set; } = false;
    public bool IsPRORFinished { get; set; } = false;

    public Operation CurrentOperation { get; set; }
    public Status CurrentStatus { get; set; }

    public int ImagesToProcess { get; set; }
    public int ProcessedImages { get; set; }

    public void SetFrom(ProjectProcessingStatus iProjectProcessingStatus)
    {
        CurrentOperation = iProjectProcessingStatus.Operation;
        CurrentStatus = iProjectProcessingStatus.Status;
        ImagesToProcess = iProjectProcessingStatus.ImagesToProcess;
        ProcessedImages = iProjectProcessingStatus.ProcessedImages;
        IsPRORFinished = iProjectProcessingStatus.IsPRORFinished;
    }

    public void UpdateUIIn(ImagesProcessingView iImagesProcessingView)
    {
        if (CurrentOperation == Operation.IsCopyingImages)
        {
            IsCopyingImages(iImagesProcessingView);
        }
        else if (IsFinished == true)
        {
            IsAllFinished(iImagesProcessingView);
        }
        else if (CurrentOperation == Operation.IsConvertingToGrayscale)
        {
            IsConvertingToGrayscale(iImagesProcessingView);
        }
        else if (CurrentOperation == Operation.IsDetectingObjects)
        {
            IsDetectingObjects(iImagesProcessingView);
        }
        else if (CurrentOperation == Operation.IsJsonSerializeing)
        {
            IsJsonSerializeing(iImagesProcessingView);
        }
        else if (CurrentOperation == Operation.IsPrecompiling)
        {
            IsPrecompiling(iImagesProcessingView);
        }
    }

    private void IsCopyingImages(ImagesProcessingView iImagesProcessingView)
    {
        iImagesProcessingView.Dispatcher.Invoke((Action)(() =>
        {
            iImagesProcessingView.Grid0Row1.Height = new GridLength();

            iImagesProcessingView.Grid1Row1.Height = new GridLength(0);
            iImagesProcessingView.Grid2Row1.Height = new GridLength(0);

            SetProgressbarAndCounter(iImagesProcessingView.CopyingImagesProgressbar, iImagesProcessingView.CopyingImagesCounter);
        }));
    }
    private void IsConvertingToGrayscale(ImagesProcessingView iImagesProcessingView)
    {
        iImagesProcessingView.Dispatcher.Invoke((Action)(() =>
        {
            iImagesProcessingView.Grid0ToggleButtonCheckbox.IsChecked = true;

            iImagesProcessingView.Grid1Row1.Height = new GridLength();

            iImagesProcessingView.Grid0Row1.Height = new GridLength(0);
            iImagesProcessingView.Grid2Row1.Height = new GridLength(0);

            SetProgressbarAndCounter(iImagesProcessingView.ConvertingToGrayscaleProgressbar, iImagesProcessingView.ConvertingToGrayscaleCounter);
        }));
    }
    private void IsDetectingObjects(ImagesProcessingView iImagesProcessingView)
    {
        iImagesProcessingView.Dispatcher.Invoke((Action)(() =>
        {
            iImagesProcessingView.Grid0ToggleButtonCheckbox.IsChecked = true;
            iImagesProcessingView.Grid1ToggleButtonCheckbox.IsChecked = true;

            iImagesProcessingView.Grid2Row1.Height = new GridLength();

            iImagesProcessingView.Grid0Row1.Height = new GridLength(0);
            iImagesProcessingView.Grid1Row1.Height = new GridLength(0);

            SetProgressbarAndCounter(iImagesProcessingView.DetectingObjectsProgressbar, iImagesProcessingView.DetectingObjectsCounter);
        }));
    }
    private void IsJsonSerializeing(ImagesProcessingView iImagesProcessingView)
    {
        iImagesProcessingView.Dispatcher.Invoke((Action)(() =>
        {
            iImagesProcessingView.Grid0ToggleButtonCheckbox.IsChecked = true;
            iImagesProcessingView.Grid1ToggleButtonCheckbox.IsChecked = true;
            iImagesProcessingView.Grid2ToggleButtonCheckbox.IsChecked = true;

            iImagesProcessingView.Grid0Row1.Height = new GridLength(0);
            iImagesProcessingView.Grid2Row1.Height = new GridLength(0);
            iImagesProcessingView.Grid1Row1.Height = new GridLength(0);
        }));
    }
    private void IsPrecompiling(ImagesProcessingView iImagesProcessingView)
    {
        iImagesProcessingView.Dispatcher.Invoke((Action)(() =>
        {
            iImagesProcessingView.Grid0ToggleButtonCheckbox.IsChecked = true;
            iImagesProcessingView.Grid1ToggleButtonCheckbox.IsChecked = true;
            iImagesProcessingView.Grid2ToggleButtonCheckbox.IsChecked = true;

            iImagesProcessingView.Grid3Row1.Height = new GridLength();

            iImagesProcessingView.Grid0Row1.Height = new GridLength(0);
            iImagesProcessingView.Grid1Row1.Height = new GridLength(0);
            iImagesProcessingView.Grid2Row1.Height = new GridLength(0);

            SetProgressbarAndCounter(iImagesProcessingView.PrecompilingProgressbar, iImagesProcessingView.PrecompilingCounter);
        }));
    }
    private void IsAllFinished(ImagesProcessingView iImagesProcessingView)
    {
        iImagesProcessingView.Dispatcher.Invoke((Action)(() =>
        {
            iImagesProcessingView.Grid0ToggleButtonCheckbox.IsChecked = true;
            iImagesProcessingView.Grid1ToggleButtonCheckbox.IsChecked = true;
            iImagesProcessingView.Grid2ToggleButtonCheckbox.IsChecked = true;
            iImagesProcessingView.Grid3ToggleButtonCheckbox.IsChecked = true;


            iImagesProcessingView.Grid0Row1.Height = new GridLength(0);
            iImagesProcessingView.Grid1Row1.Height = new GridLength(0);
            iImagesProcessingView.Grid2Row1.Height = new GridLength(0);
            iImagesProcessingView.Grid3Row1.Height = new GridLength(0);
        }));
    }


    private int GetPercents(int imagesToProcess, int processedImages)
    {
        if (imagesToProcess == 0 || processedImages == 0)
        {
            return 0;
        }
        else
        {
            decimal percent = (decimal)imagesToProcess / 100;
            decimal result = processedImages / percent;

            return (int)result;
        }
    }

    private void SetProgressbarAndCounter(ProgressBar iProgressBar, TextBlock iCounter)
    {
        iProgressBar.Value = GetPercents(ImagesToProcess, ProcessedImages);
        iCounter.Text = $"{ProcessedImages}/{ImagesToProcess}";
    }

    internal void Set(Operation isPrecompiling, int count)
    {
        CurrentOperation = isPrecompiling;
        ImagesToProcess = count;
    }
}
