//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System;

namespace Carbon.Recognition.Translation
{
    /// <summary>
    /// Define payload of translation synthesis result events.
    /// </summary>
    public class TranslationSynthesisResultEventArgs : System.EventArgs
    {
        internal TranslationSynthesisResultEventArgs(Carbon.Internal.TranslationSynthesisResultEventArgs e)
        {
            this.Result = new TranslationSynthesisResult(e.Result);
            this.SessionId = e.SessionId;
        }

        /// <summary>
        /// Specifies the translation synthesis result.
        /// </summary>
        public TranslationSynthesisResult Result { get; }

        /// <summary>
        /// Specifies the session identifier.
        /// </summary>
        public string SessionId { get; }

        /// <summary>
        /// Returns a string that represents the speech recognition result event.
        /// </summary>
        /// <returns>A string that represents the speech recognition result event.</returns>
        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
