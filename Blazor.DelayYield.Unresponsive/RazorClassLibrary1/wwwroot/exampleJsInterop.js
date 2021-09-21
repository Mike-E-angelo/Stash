// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

// ReSharper disable once Es6Feature
export function showPrompt(message) {
  return prompt(message, 'Type anything here');
}
