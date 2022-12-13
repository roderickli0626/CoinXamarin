using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using mycoin.Droid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Xamarin.Forms.TimePicker), typeof(TimePickerDialogCustomRenderer))]
namespace mycoin.Droid
{
    public class TimePickerDialogCustomRenderer : TimePickerRenderer
    {
        private readonly Context _context;
        public TimePickerDialogCustomRenderer(Context context) : base(context)
        {
            _context = context;

        }
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.TimePicker> e)
        {
            base.OnElementChanged(e);
        }

        TimePickerDialog dialog;
        protected override TimePickerDialog CreateTimePickerDialog(int hours, int minutes)
        {
            dialog = new TimePickerDialog(_context, Resource.Style.TimePickerTheme, this, hours, minutes, false);

            return dialog;

        }
    }
}