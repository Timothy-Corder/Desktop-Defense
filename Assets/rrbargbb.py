from PIL import Image

def rgba_to_rgbb(input_path, output_path):
    # Open the image
    img = Image.open(input_path).convert("RGBA")

    # Extract pixel data
    pixels = img.load()

    # Create a new image with the same size
    width, height = img.size
    new_img = Image.new("RGBA", (width, height))
    new_pixels = new_img.load()

    for x in range(width):
        for y in range(height):
            r, g, b, a = pixels[x, y]  # Get RGBA values
            new_pixels[x, y] = (r, g, a, b)  # Replace B with A

    # Save the transformed image
    new_img.save(output_path)
    print(f"Image saved as {output_path}")

# Example usage
input_image = "Anchor.png"  # Replace with your input image path
output_image = "ancr.png"  # Replace with your desired output path
rgba_to_rgbb(input_image, output_image)
