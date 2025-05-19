window.downloadFile = async (fileName, fileBytes) => {
    const blob = new Blob([new Uint8Array(fileBytes)])
    const url = URL.createObjectURL(blob)
    const anchorElement = document.createElement('a')

    anchorElement.href = url
    anchorElement.download = fileName ?? ''
    anchorElement.click()
    anchorElement.remove()
    URL.revokeObjectURL(url)
}