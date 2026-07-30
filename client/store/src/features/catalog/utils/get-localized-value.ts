export function getLocalizedValue(
  language: string,
  englishValue: string,
  arabicValue: string
) {
  return language.startsWith("ar")
    ? arabicValue || englishValue
    : englishValue || arabicValue
}
