/*  CTRADER GURU --> Indicator Template 1.0.6

    Homepage    : https://ctrader.guru/
    Telegram    : https://t.me/ctraderguru
    Twitter     : https://twitter.com/cTraderGURU/
    Facebook    : https://www.facebook.com/ctrader.guru/
    YouTube     : https://www.youtube.com/channel/UCKkgbw09Fifj65W5t5lHeCQ
    GitHub      : https://github.com/ctrader-guru

*/

using System;
using cAlgo.API;
using System.Linq;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo
{

    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Trendy : Indicator
    {

        #region Enums

        // --> Eventuali enumeratori li mettiamo qui

        #endregion

        #region Identity

        /// <summary>
        /// Nome del prodotto, identificativo, da modificare con il nome della propria creazione
        /// </summary>
        public const string NAME = "Trendy";

        /// <summary>
        /// La versione del prodotto, progressivo, utilie per controllare gli aggiornamenti se viene reso disponibile sul sito ctrader.guru
        /// </summary>
        public const string VERSION = "1.0.1";

        #endregion

        #region Params

        /// <summary>
        /// Identità del prodotto nel contesto di ctrader.guru
        /// </summary>
        [Parameter(NAME + " " + VERSION, Group = "Identity", DefaultValue = "https://ctrader.guru/product/indicator-base/")]
        public string ProductInfo { get; set; }

        [Parameter("MA Period", Group = "Params", DefaultValue = 24)]
        public int MaPeriod { get; set; }

        [Parameter("MA Type", Group = "Params", DefaultValue = MovingAverageType.Exponential)]
        public MovingAverageType MAType { get; set; }

        [Output("Bullish", LineColor = "LimeGreen", IsHistogram = true, LineStyle = LineStyle.Solid, Thickness = 5)]
        public IndicatorDataSeries Bullish { get; set; }

        [Output("Bearish", LineColor = "Red", IsHistogram = true, LineStyle = LineStyle.Solid, Thickness = 5)]
        public IndicatorDataSeries Bearish { get; set; }

        [Output("Flat", LineColor = "Orange", IsHistogram = true, LineStyle = LineStyle.Solid, Thickness = 5)]
        public IndicatorDataSeries Flat { get; set; }

        #endregion

        #region Property

        private MovingAverage _ma;
        private ParabolicSAR _sar;

        private readonly Random _random = new Random();
        
        #endregion

        #region Indicator Events

        /// <summary>
        /// Viene generato all'avvio dell'indicatore, si inizializza l'indicatore
        /// </summary>
        protected override void Initialize()
        {

            // --> Stampo nei log la versione corrente
            Print("{0} : {1}", NAME, VERSION);

            _ma = Indicators.MovingAverage(Bars.ClosePrices, MaPeriod, MAType);
            _sar = Indicators.ParabolicSAR(Bars, 0.02, 0.2);

        }

        /// <summary>
        /// Generato ad ogni tick, vengono effettuati i calcoli dell'indicatore
        /// </summary>
        /// <param name="index">L'indice della candela in elaborazione</param>
        public override void Calculate(int index)
        {

            _drawBar(Bars, _ma, _sar, 0, 1, index);

        }

        #endregion

        #region Private Methods


        private void _drawBar(Bars myMS, MovingAverage myMA, ParabolicSAR mySAR, double start, double stop, int index)
        {

            double close = myMS.ClosePrices[index];

            double MA = myMA.Result[index];
            int directcion = 0;

            if (MA < close)
            {

                directcion = (close > mySAR.Result.LastValue) ? 1 : 0;

            }
            else if( MA > close ){

                directcion = (close < mySAR.Result.LastValue) ? -1 : 0;

            }

            switch (directcion)
            {

                case 1:

                    Bullish[index] = 2;
                    Flat[index] = 0;
                    Bearish[index] = 0;

                    break;

                case -1:

                    Bearish[index] = 2;
                    Bullish[index] = 0;
                    Flat[index] = 0;

                    break;

                default:

                    Flat[index] = 2;
                    Bullish[index] = 0;
                    Bearish[index] = 0;

                    break;

            }

        }

        #endregion

    }

}