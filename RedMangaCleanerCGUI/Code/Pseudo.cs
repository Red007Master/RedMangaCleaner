using System.Threading;

namespace RedMangaCleanerCGUI.Code.Xaml.Windows
{
    class Pseudo
    {
        public static void PseudoMain()
        {
            WPFGate.SetLoadingTextBox("NetworkCheck");
            Web.NetworkCheck();

            WPFGate.SetLoadingTextBox("Initalization");
            Initalization.Start();

            if (/*!*/P.Config.UserAgreementIsAccepted) //DEV
            {
                P.Logger.Log("UA don't accepted, opening UserAgreementAndFirstInitWindow", LogLevel.Information, 1);
                WPFGate.AsUserAgreementDontAccepted();

                while (!P.Config.UserAgreementIsAccepted) //TODO replace this dogshit
                {
                    Thread.Sleep(100);
                }
            }

            WPFGate.OpenMainWindow();

            while (true)
            {
                Thread.Sleep(100);
            }

            //Work.MainVoid();
        }
    }
}