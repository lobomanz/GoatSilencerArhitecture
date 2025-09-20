tinymce.init({
    selector: 'textarea[name="RichTextContent"]',
    height: 500,
    menubar: false,
    plugins: 'link image lists code table',
    toolbar: 'undo redo | blocks | bold italic underline | alignleft aligncenter alignright | bullist numlist | link image | code',
    block_formats: 'Paragraph=p; Heading 1=h1; Heading 2=h2; Heading 3=h3; Heading 4=h4',
    content_style: 'body { font-family: Arial, sans-serif; line-height: 1.6; font-size: 16px; }'
});
