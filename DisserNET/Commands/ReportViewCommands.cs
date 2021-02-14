using DisserNET.Calculs;
using DisserNET.Models;
using DisserNET.ViewModels;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DisserNET.Commands
{
    abstract public class ReportViewCommands : ICommand
    {
        protected ReportViewModel rvm;
        public ReportViewCommands(ReportViewModel reportViewModel)
        {
            this.rvm = reportViewModel;
        }

        public event EventHandler CanExecuteChanged;

        public abstract bool CanExecute(object parameter);

        public abstract void Execute(object parameter);
    }

    public class SelectFolderCommand : ReportViewCommands
    {
        public SelectFolderCommand(ReportViewModel vm) : base(vm)
        {

        }

        public override bool CanExecute(object parameter) => true;

        public override void Execute(object parameter)
        {
            using var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = true,
                DefaultDirectory = rvm.BaseReportDir
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folder = dialog.FileNames.FirstOrDefault();
                if (folder != null)
                {
                    var files = Directory.EnumerateFiles(folder);
                    var mhParams = files.FirstOrDefault(s => s.EndsWith(rvm.reportDb.MhParamsNameAndExt));
                    var mhRes = files.FirstOrDefault(s => s.EndsWith(rvm.reportDb.MhAcceptedNameAndExt));
                    var mhComment = files.FirstOrDefault(s => s.EndsWith(rvm.reportDb.MhCommentNameAndExt));

                    if (mhComment == null)
                    {
                        mhComment = Path.Combine(Path.GetDirectoryName(mhParams), rvm.reportDb.MhCommentNameAndExt);
                        //File.Create(mhComment);
                        //File.
                    }

                    rvm.ReportModel = new ReportModel(mhParams, mhRes, mhComment);
                }
            }
        }
    }
}
