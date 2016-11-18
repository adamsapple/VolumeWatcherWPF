using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using Audio.CoreAudio.Interfaces;
using Audio.DMO.Interfaces;

namespace Audio.DMO
{
    /// <summary>
    /// From wmcodecsdp.h
    /// Implements:
    /// - IMediaObject 
    /// - IMFTransform (Media foundation - we will leave this for now as there is loads of MF stuff)
    /// - IPropertyStore 
    /// - IWMResamplerProps 
    /// Can resample PCM or IEEE
    /// </summary>
    [ComImport, Guid(ComIds.CLSID_RESAMPLER_MFT)]
    class ResamplerMediaComObject
    {
    }

    /// <summary>
    /// Resampler
    /// </summary>
    public class Resampler : IDisposable
    {
        MediaObject mediaObject;
        IPropertyStore propertyStoreInterface;
        IWMResamplerProps resamplerPropsInterface;
        ResamplerMediaComObject mediaComObject;

        /// <summary>
        /// Creates a new Resampler based on the DMO Resampler
        /// </summary>
        public Resampler()
        {
            mediaComObject = new ResamplerMediaComObject();
            mediaObject = new MediaObject((IMediaObject)mediaComObject);
            propertyStoreInterface = (IPropertyStore)mediaComObject;
            resamplerPropsInterface = (IWMResamplerProps)mediaComObject;
        }

        /// <summary>
        /// Media Object
        /// </summary>
        public MediaObject MediaObject
        {
            get
            {
                return mediaObject;
            }
        }


        #region IDisposable Members

        /// <summary>
        /// Dispose code - experimental at the moment
        /// Was added trying to track down why Resampler crashes NUnit
        /// This code not currently being called by ResamplerDmoStream
        /// </summary>
        public void Dispose()
        {
            if(propertyStoreInterface != null)
            {
                Marshal.ReleaseComObject(propertyStoreInterface);
                propertyStoreInterface = null;
            }
            if(resamplerPropsInterface != null)
            {
                Marshal.ReleaseComObject(resamplerPropsInterface);
                resamplerPropsInterface = null;
            }
            if (mediaObject != null)
            {
                mediaObject.Dispose();
                mediaObject = null;
            }
            if (mediaComObject != null)
            {
                Marshal.ReleaseComObject(mediaComObject);
                mediaComObject = null;
            }
        }

        #endregion
    }
}
